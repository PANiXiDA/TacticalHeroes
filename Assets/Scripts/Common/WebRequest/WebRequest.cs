using Assets.Scripts.Common.Enumerations;
using Assets.Scripts.Common.WebRequest.JWT;
using Assets.Scripts.Common.WebRequest.Responses;
using Cysharp.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Common.WebRequest
{
    public static class UniversalWebRequest
    {
        private const string _authScene = "Auth";

        private const string _domen = "https://tacticalheroesdev.ru/api/v1/";

        public static async UniTask<RestApiResponse<TResponse>> SendRequest<TRequest, TResponse>(string apiUrl, RequestType requestType, TRequest modelRequest)
        {
            string stringRequest = JsonConvert.SerializeObject(modelRequest);
            byte[] byteRequest = Encoding.UTF8.GetBytes(stringRequest);

            string fullUrl = _domen + apiUrl;

            using (UnityWebRequest request = new UnityWebRequest(fullUrl, requestType.ToString()))
            {
                request.uploadHandler = new UploadHandlerRaw(byteRequest);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                string accessToken = JwtToken.AccessToken;
                if (!string.IsNullOrEmpty(accessToken))
                {
                    request.SetRequestHeader("Authorization", $"Bearer {accessToken}");
                }

                await request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    string jsonResponse = request.downloadHandler.text;

                    try
                    {
                        return JsonConvert.DeserializeObject<RestApiResponse<TResponse>>(jsonResponse)
                            ?? RestApiResponse<TResponse>.Fail(new Failure().AddError("Empty response"));
                    }
                    catch (JsonException ex)
                    {
                        return RestApiResponse<TResponse>.Fail(new Failure().AddError($"Failed to parse response: {ex.Message}"));
                    }
                }
                else if (request.responseCode == 401)
                {
                    var refreshResponse = await SendRequest<RefreshTokenRequest, RefreshTokenResponse>(apiUrl, requestType, new RefreshTokenRequest() { RefreshToken = JwtToken.RefreshToken }); ;
                    if (refreshResponse.IsSuccess)
                    {
                        JwtToken.AccessToken = refreshResponse.Payload.AccessToken;
                        JwtToken.RefreshToken = refreshResponse.Payload.RefreshToken;

                        return await SendRequest<TRequest, TResponse>(apiUrl, requestType, modelRequest);
                    }
                    SceneManager.LoadScene(_authScene);

                    return RestApiResponse<TResponse>.Fail(new Failure().AddError("Unauthorized."));

                }
                else
                {
                    return RestApiResponse<TResponse>.Fail(new Failure().AddError(request.error));
                }
            }
        }
    }
}

