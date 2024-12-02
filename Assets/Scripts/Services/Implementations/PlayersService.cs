using Assets.Scripts.Common.Constants;
using Assets.Scripts.Common.Enumerations;
using Assets.Scripts.Common.WebRequest;
using Assets.Scripts.Services.Interfaces;
using Cysharp.Threading.Tasks;
using Assets.Scripts.Infrastructure.Requests;
using Assets.Scripts.Infrastructure.Responses.PlayersService;

namespace Assets.Scripts.Services.Implementations
{
    public class PlayersService : IPlayersService
    {
        public PlayersService() { }

        public async UniTask<GetPlayerProfileResponse> GetPlayerProfile(EmptyRequest request)
        {
            var result = await UniversalWebRequest.SendRequest<EmptyRequest, GetPlayerProfileResponse>(
                ApiEndpointsConstants.GetPlayerEndpoint,
                RequestType.GET,
                request);

            return result.EnsureSuccess();
        }
    }
}
