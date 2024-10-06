using Assets.Scripts.Integrations.Firebase.Infrastructure.Requests;
using System;

namespace Assets.Scripts.Integrations.Firebase.Interfaces
{
    public interface IFirebaseChatService
    {
        void SendChatMessage(SaveChatMessageRequest message);
        void SubscribeToNewMessages(string chatId, Action<SaveChatMessageRequest> onNewMessage);
    }
}
