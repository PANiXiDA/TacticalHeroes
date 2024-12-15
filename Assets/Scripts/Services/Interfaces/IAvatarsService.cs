using Assets.Scripts.Infrastructure.Requests;
using Assets.Scripts.Infrastructure.Responses.AvatarsService;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace Assets.Scripts.Services.Interfaces
{
    public interface IAvatarsService
    {
        UniTask<List<GetAvailableAvatarsResponse>> GetAvailable(EmptyRequest request);
    }
}
