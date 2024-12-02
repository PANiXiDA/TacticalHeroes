using Assets.Scripts.Infrastructure.Requests;
using Assets.Scripts.Infrastructure.Responses.PlayersService;
using Cysharp.Threading.Tasks;

namespace Assets.Scripts.Services.Interfaces
{
    public interface IPlayersService
    {
        UniTask<GetPlayerProfileResponse> GetPlayerProfile(EmptyRequest request);
    }
}
