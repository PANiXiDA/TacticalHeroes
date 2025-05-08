using Assets.Scripts.Infrastructure.Requests;
using Assets.Scripts.Infrastructure.Requests.ChatsService;
using Assets.Scripts.Infrastructure.Responses.ChatsService;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Services.Interfaces
{
    public interface IChatsService
    {
        event Action<GlobalChatMessageResponse> OnGlobalChatMessageReceived;

        UniTask<List<GlobalChatMessageResponse>> GetGlobalChatHistory(EmptyRequest request);
        UniTask ConnectToGlobalChat();
        UniTask SendGlobalChatMessage(GlobalChatMessageRequest request);
        UniTask DisconnectFromGlobalChat();
    }
}
