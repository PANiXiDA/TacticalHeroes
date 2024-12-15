using Assets.Scripts.Common.Constants;
using Assets.Scripts.Common.Enumerations;
using Assets.Scripts.Common.WebRequest;
using Assets.Scripts.Infrastructure.Requests;
using Assets.Scripts.Infrastructure.Responses.FramesService;
using Assets.Scripts.Services.Interfaces;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace Assets.Scripts.Services.Implementations
{
    public class FramesService : IFramesService
    {
        public FramesService() { }

        public async UniTask<List<GetAvailableFramesResponse>> GetAvailable(EmptyRequest request)
        {
            var result = await UniversalWebRequest.SendRequest<EmptyRequest, List<GetAvailableFramesResponse>>(
                ApiEndpointsConstants.GetAvailableFramesEndpoint,
                RequestType.GET,
                request);

            return result.EnsureSuccess();
        }
    }
}
