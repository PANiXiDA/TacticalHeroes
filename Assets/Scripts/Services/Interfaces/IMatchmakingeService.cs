using Assets.Scripts.Infrastructure.Responses.MatchmakingeService;
using Cysharp.Threading.Tasks;
using System;

namespace Assets.Scripts.Services.Interfaces
{
    public interface IMatchmakingeService
    {
        event Action<MatchmakingResponse> OnMatchFound;
        UniTask ConnectToMatchmaking();
        UniTask DisconnectFromMatchmaking();
    }
}
