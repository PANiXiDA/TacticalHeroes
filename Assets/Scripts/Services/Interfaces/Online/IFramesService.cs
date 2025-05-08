using Assets.Scripts.Infrastructure.Requests;
using Assets.Scripts.Infrastructure.Responses.FramesService;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace Assets.Scripts.Services.Interfaces
{
    public interface IFramesService
    {
        UniTask<List<GetAvailableFramesResponse>> GetAvailable(EmptyRequest request);
    }
}
