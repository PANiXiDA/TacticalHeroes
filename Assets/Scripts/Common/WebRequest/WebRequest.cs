using Assets.Scripts.Common.Enumerations;
using Assets.Scripts.Common.WebRequest.Responses;
using Cysharp.Threading.Tasks;
using System.Text;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine.Networking;

namespace Assets.Scripts.Common.WebRequest
{
    public static class UniversalWebRequest
    {
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

                await request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    string jsonResponse = request.downloadHandler.text;

                    try
                    {
                        var response = JsonConvert.DeserializeObject<RestApiResponse<TResponse>>(jsonResponse);

                        return response ?? RestApiResponse<TResponse>.Fail(new Failure().AddError("Empty response"));
                    }
                    catch (JsonException ex)
                    {
                        return RestApiResponse<TResponse>.Fail(new Failure().AddError($"Failed to parse response: {ex.Message}"));
                    }
                }
                else
                {
                    string errorMessage = $"Request failed with error: {request.error}";

                    return RestApiResponse<TResponse>.Fail(new Failure().AddError(errorMessage));
                }
            }
        }
    }
}
