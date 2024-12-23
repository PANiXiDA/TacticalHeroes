using Assets.Scripts.Infrastructure.Requests.ChatsService;
using Assets.Scripts.Infrastructure.Responses.ChatsService;
using Assets.Scripts.Services.Interfaces;
using Cysharp.Threading.Tasks;
using System;
using Assets.Scripts.Common.Constants;
using Assets.Scripts.Common.Enumerations;
using Assets.Scripts.Common.WebRequest;
using Assets.Scripts.Infrastructure.Requests;
using System.Collections.Generic;

public class ChatsService : IChatsService
{
    public event Action<GlobalChatMessageResponse> OnGlobalChatMessageReceived;

    public async UniTask<List<GlobalChatMessageResponse>> GetGlobalChatHistory(EmptyRequest request)
    {
        var result = await UniversalWebRequest.SendRequest<EmptyRequest, List<GlobalChatMessageResponse>> (
            ApiEndpointsConstants.GlobalChatHistoryEndpoint,
            RequestType.GET,
            request);

        return result.EnsureSuccess();
    }

    public async UniTask ConnectToGlobalChat()
    {
        await UniversalWebSocket.ConnectAsync<GlobalChatMessageResponse>(
            ApiEndpointsConstants.GlobalChatEndpoint,
            HandleGlobalChatMessageReceived
        );
    }

    public async UniTask SendGlobalChatMessage(GlobalChatMessageRequest request)
    {
        await UniversalWebSocket.SendMessageAsync(ApiEndpointsConstants.GlobalChatEndpoint, request);
    }

    public async UniTask DisconnectFromGlobalChat()
    {
        await UniversalWebSocket.DisconnectAsync(ApiEndpointsConstants.GlobalChatEndpoint);
    }

    private void HandleGlobalChatMessageReceived(GlobalChatMessageResponse response)
    {
        OnGlobalChatMessageReceived?.Invoke(response);
    }
}