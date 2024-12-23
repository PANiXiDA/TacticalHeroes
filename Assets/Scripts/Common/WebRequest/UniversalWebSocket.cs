using Assets.Scripts.Common.WebRequest.JWT;
using Cysharp.Threading.Tasks;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System;
using UnityEngine;
using Newtonsoft.Json;

public class UniversalWebSocket
{
    private const string Domain = "wss://tacticalheroesdev.ru/api/v1/";

    private static readonly ConcurrentDictionary<string, ClientWebSocket> WebSocketConnections = new ConcurrentDictionary<string, ClientWebSocket>();
    private static readonly ConcurrentDictionary<string, ConcurrentQueue<object>> MessageQueues = new ConcurrentDictionary<string, ConcurrentQueue<object>>();
    private static readonly ConcurrentDictionary<string, CancellationTokenSource> CancellationTokens = new ConcurrentDictionary<string, CancellationTokenSource>();

    public static async UniTask ConnectAsync<TResponse>(
        string endpoint,
        Action<TResponse> onResponseReceived)
    {
        var fullEndpoint = CombineEndpoint(endpoint);

        if (WebSocketConnections.ContainsKey(fullEndpoint))
        {
            Debug.LogWarning($"WebSocket уже подключен к: {fullEndpoint}");
            return;
        }

        var clientWebSocket = new ClientWebSocket();
        var cancellationTokenSource = new CancellationTokenSource();

        try
        {
            if (!WebSocketConnections.TryAdd(fullEndpoint, clientWebSocket))
            {
                Debug.LogError($"Не удалось добавить WebSocket для: {fullEndpoint}");
                throw new InvalidOperationException();
            }

            if (!CancellationTokens.TryAdd(fullEndpoint, cancellationTokenSource))
            {
                Debug.LogError($"Не удалось добавить CancellationToken для: {fullEndpoint}");
                throw new InvalidOperationException();
            }

            string accessToken = JwtTokenManager.AccessToken;
            if (!string.IsNullOrEmpty(accessToken))
            {
                clientWebSocket.Options.SetRequestHeader("Authorization", $"Bearer {accessToken}");
            }

            await clientWebSocket.ConnectAsync(new Uri(fullEndpoint), cancellationTokenSource.Token);

            MessageQueues[fullEndpoint] = new ConcurrentQueue<object>();

            var sendTask = SendLoop(fullEndpoint, cancellationTokenSource.Token);
            var receiveTask = ReceiveLoop(fullEndpoint, onResponseReceived, cancellationTokenSource.Token);

            await UniTask.WhenAny(sendTask, receiveTask);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Ошибка подключения к WebSocket: {ex.Message}");
            if (clientWebSocket.State == WebSocketState.Open)
            {
                await clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Закрытие соединения", cancellationTokenSource.Token);
            }

            clientWebSocket.Dispose();
            CancellationTokens.TryRemove(fullEndpoint, out _);

            throw;
        }
    }

    public static UniTask SendMessageAsync<TRequest>(string endpoint, TRequest message)
    {
        var fullEndpoint = CombineEndpoint(endpoint);

        if (!WebSocketConnections.TryGetValue(fullEndpoint, out var clientWebSocket))
        {
            Debug.LogError($"SendMessageAsync: Соединение WebSocket не найдено для: {fullEndpoint}");
            throw new InvalidOperationException();
        }

        if (!MessageQueues.TryGetValue(fullEndpoint, out var messageQueue))
        {
            messageQueue = new ConcurrentQueue<object>();
            MessageQueues[fullEndpoint] = messageQueue;
        }

        messageQueue.Enqueue(message);

        return UniTask.CompletedTask;
    }

    public static async UniTask DisconnectAsync(string endpoint)
    {
        var fullEndpoint = CombineEndpoint(endpoint);

        CancellationToken cancellationToken = CancellationToken.None;

        if (CancellationTokens.TryRemove(fullEndpoint, out var cancellationTokenSource))
        {
            cancellationTokenSource.Cancel();
            cancellationToken = cancellationTokenSource.Token;
            cancellationTokenSource.Dispose();
        }

        if (WebSocketConnections.TryRemove(fullEndpoint, out var clientWebSocket))
        {
            if (clientWebSocket.State == WebSocketState.Open)
            {
                await clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing connection", cancellationToken);
            }

            clientWebSocket.Dispose();
            MessageQueues.TryRemove(fullEndpoint, out _);
        }
        else
        {
            Debug.LogWarning($"No active WebSocket connection found for endpoint: {fullEndpoint}");
            throw new InvalidOperationException();
        }
    }


    private static string CombineEndpoint(string endpoint)
    {
        if (Uri.TryCreate(endpoint, UriKind.Absolute, out _))
        {
            return endpoint;
        }

        return Domain.TrimEnd('/') + "/" + endpoint.TrimStart('/');
    }

    private static async UniTask SendLoop(string endpoint, CancellationToken cancellationToken)
    {
        if (!WebSocketConnections.TryGetValue(endpoint, out var clientWebSocket))
        {
            Debug.LogError($"SendLoop: Соединение WebSocket не найдено для: {endpoint}");
            throw new InvalidOperationException();
        }

        if (!MessageQueues.TryGetValue(endpoint, out var messageQueue))
        {
            messageQueue = new ConcurrentQueue<object>();
            MessageQueues[endpoint] = messageQueue;
        }

        while (!cancellationToken.IsCancellationRequested && clientWebSocket.State == WebSocketState.Open)
        {
            try
            {
                while (messageQueue.TryDequeue(out var message))
                {
                    string jsonRequest = JsonConvert.SerializeObject(message);
                    byte[] requestBytes = Encoding.UTF8.GetBytes(jsonRequest);

                    await clientWebSocket.SendAsync(new ArraySegment<byte>(requestBytes), WebSocketMessageType.Text, true, cancellationToken);
                }

                await UniTask.Delay(50, cancellationToken: cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Ошибка в SendLoop для {endpoint}: {ex.Message}");
                throw;
            }
        }
    }

    private static async UniTask ReceiveLoop<TResponse>(
        string endpoint,
        Action<TResponse> onResponseReceived,
        CancellationToken cancellationToken)
    {
        if (!WebSocketConnections.TryGetValue(endpoint, out var clientWebSocket))
        {
            Debug.LogError($"ReceiveLoop: Соединение WebSocket не найдено для: {endpoint}");
            throw new InvalidOperationException();
        }

        var buffer = new byte[1024 * 4];

        while (!cancellationToken.IsCancellationRequested && clientWebSocket.State == WebSocketState.Open)
        {
            try
            {
                var result = await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    Debug.Log($"WebSocket закрыт сервером для {endpoint}");
                    throw new InvalidOperationException();
                }

                string jsonResponse = Encoding.UTF8.GetString(buffer, 0, result.Count);
                var response = JsonConvert.DeserializeObject<TResponse>(jsonResponse);

                if (response != null)
                {
                    onResponseReceived(response);
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Ошибка в ReceiveLoop для {endpoint}: {ex.Message}");
                throw;
            }
        }
    }
}