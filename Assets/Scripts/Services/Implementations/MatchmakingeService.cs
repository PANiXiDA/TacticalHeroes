using Assets.Scripts.Common.Constants;
using Assets.Scripts.Infrastructure.Responses.MatchmakingeService;
using Assets.Scripts.Services.Interfaces;
using Cysharp.Threading.Tasks;
using System;

namespace Assets.Scripts.Services.Implementations
{
    public class MatchmakingeService : IMatchmakingeService
    {
        public event Action<MatchmakingResponse> OnMatchFound;

        public async UniTask ConnectToMatchmaking()
        {
            await UniversalWebSocket.ConnectAsync<MatchmakingResponse>(
                ApiEndpointsConstants.MatchmakingEndpoint,
                HandleMatchFound
            );
        }

        public async UniTask DisconnectFromMatchmaking()
        {
            await UniversalWebSocket.DisconnectAsync(ApiEndpointsConstants.MatchmakingEndpoint);
        }

        private void HandleMatchFound(MatchmakingResponse response)
        {
            OnMatchFound?.Invoke(response);
        }
    }
}