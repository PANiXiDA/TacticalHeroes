using Assets.Scripts.Common.Constants;
using Assets.Scripts.Common.Enumerations;
using Assets.Scripts.Common.WebRequest;
using Assets.Scripts.Infrastructure.Requests;
using Assets.Scripts.Infrastructure.Responses.AvatarsService;
using Assets.Scripts.Services.Interfaces;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace Assets.Scripts.Services.Implementations
{
    public class AvatarsService : IAvatarsService
    {
        public AvatarsService() { }

        public async UniTask<List<GetAvailableAvatarsResponse>> GetAvailable(EmptyRequest request)
        {
            var result = await UniversalWebRequest.SendRequest<EmptyRequest, List<GetAvailableAvatarsResponse>>(
                ApiEndpointsConstants.GetAvailableAvatarsEndpoint,
                RequestType.GET,
                request);

            return result.EnsureSuccess();
        }
    }
}
