using Assets.Scripts.Common.Enumerations;
using Assets.Scripts.Common.WebRequest.JWT;
using Assets.Scripts.Common.WebRequest.Responses;
using Cysharp.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Assets.Scripts.Common.Constants;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Assets.Scripts.Common.WebRequest
{
    public static class UniversalWebRequest
    {
        private const string Domen = "https://tacticalheroesdev.ru/api/v1/";

        public static async UniTask<RestApiResponse<TResponse>> SendRequest<TRequest, TResponse>(
            string apiUrl,
            RequestType requestType,
            TRequest modelRequest,
            bool isRetry = false)
        {
            string stringRequest = JsonConvert.SerializeObject(modelRequest);
            byte[] byteRequest = Encoding.UTF8.GetBytes(stringRequest);

            string fullUrl = Domen + apiUrl;

            using (UnityWebRequest request = new UnityWebRequest(fullUrl, requestType.ToString()))
            {
                request.uploadHandler = new UploadHandlerRaw(byteRequest);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                string accessToken = JwtTokenManager.AccessToken;
                if (!string.IsNullOrEmpty(accessToken))
                {
                    request.SetRequestHeader("Authorization", $"Bearer {accessToken}");
                }

                try
                {
                    await request.SendWebRequest();
                    string jsonResponse = request.downloadHandler.text;

                    return ParseResponse<TResponse>(jsonResponse);
                }
                catch
                {
                    string errorText = request.downloadHandler.text;
                    if (request.responseCode != 401 || isRetry)
                    {
                        Debug.LogError($"{request.error} : {errorText}");
                    }

                    var response = ParseResponse<TResponse>(errorText);

                    if (!isRetry)
                    {
                        if (request.responseCode == 401)
                        {
                            var refreshResponse = await SendRequest<RefreshTokenRequest, RefreshTokenResponse>(
                                ApiEndpointsConstants.RefreshTokensEndpoint,
                                RequestType.POST,
                                new RefreshTokenRequest { RefreshToken = JwtTokenManager.LoadRefreshToken() },
                                true);

                            if (refreshResponse.IsSuccess)
                            {
                                JwtTokenManager.AccessToken = refreshResponse.Payload.AccessToken;
                                JwtTokenManager.SaveRefreshToken(refreshResponse.Payload.RefreshToken);

                                return await SendRequest<TRequest, TResponse>(
                                    apiUrl,
                                    requestType,
                                    modelRequest,
                                    true);
                            }
                            else
                            {
                                SceneManager.LoadScene(SceneConstants.AuthScene);
                                return response;
                            }
                        }
                        return response;
                    }
                    else
                    {
                        return response;
                    }
                }
            }
        }

        private static RestApiResponse<TResponse> ParseResponse<TResponse>(string responseText)
        {
            try
            {
                var response = JsonConvert.DeserializeObject<RestApiResponse<TResponse>>(responseText)
                    ?? RestApiResponse<TResponse>.Fail(new Failure().AddError("Empty response"));

                if (response.IsSuccess)
                {
                    return response;
                }

                var processedErrors = new Dictionary<string, string>();
                foreach (var error in response.Failure.Errors)
                {
                    var match = Regex.Match(error.Value, @"Detail=\""(.*?)\""");
                    processedErrors[error.Key] = match.Success ? match.Groups[1].Value : error.Value;
                }

                response.Failure.Errors.Clear();
                foreach (var error in processedErrors)
                {
                    response.Failure.Errors[error.Key] = error.Value;
                }

                return response;
            }
            catch (JsonException ex)
            {
                return RestApiResponse<TResponse>.Fail(new Failure().AddError($"Failed to parse response: {ex.Message}"));
            }
        }
    }
}