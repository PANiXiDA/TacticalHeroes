using Assets.Scripts.Common.Helpers;
using Assets.Scripts.Infrastructure.Requests;
using Assets.Scripts.Infrastructure.Requests.ChatsService;
using Assets.Scripts.Infrastructure.Requests.ImageService;
using Assets.Scripts.Infrastructure.Responses.ChatsService;
using Assets.Scripts.Services.Interfaces;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ChatManager : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField _input;

    [SerializeField]
    private GameObject _messagePrefab;

    [SerializeField]
    private Transform _chat;

    [SerializeField]
    private ScrollRect _scrollRect;

    private IChatsService _chatsService;
    private IImagesService _imagesService;

    [Inject]
    public void Constructor(
        IChatsService chatsService,
        IImagesService imagesService)
    {
        _chatsService = chatsService;
        _imagesService = imagesService;
    }

    private async void Start()
    {
        await ConnectToGlobalChat();
    }

    private async void OnDestroy()
    {
        await DisconnectFromGlobalChat();
    }

    private async UniTask ConnectToGlobalChat()
    {
        await TaskRunner.RunWithGlobalErrorHandling(async () =>
        {
            var messageHistory = await _chatsService.GetGlobalChatHistory(new EmptyRequest());
            foreach (var message in messageHistory)
            {
                OnMessageReceived(message);
            }

            _chatsService.OnGlobalChatMessageReceived += OnMessageReceived;
            await _chatsService.ConnectToGlobalChat();
        });
    }

    private async UniTask DisconnectFromGlobalChat()
    {
        await TaskRunner.RunWithGlobalErrorHandling(async () =>
        {
            _chatsService.OnGlobalChatMessageReceived -= OnMessageReceived;
            await _chatsService.DisconnectFromGlobalChat();
        });
    }

    public async void SendMessageToChat()
    {
        await TaskRunner.RunWithGlobalErrorHandling(async () =>
        {
            var request = new GlobalChatMessageRequest(_input.text);
            await _chatsService.SendGlobalChatMessage(request);

            _input.text = string.Empty;
        });
    }

    private async void OnMessageReceived(GlobalChatMessageResponse message)
    {
        GameObject newMessage = Instantiate(_messagePrefab, _chat);

        TextMeshProUGUI nicknameText = newMessage.transform.Find("NickName").GetComponent<TextMeshProUGUI>();
        nicknameText.text = message.SenderNickname;

        TextMeshProUGUI messageText = newMessage.transform.Find("Message/Text").GetComponent<TextMeshProUGUI>();
        messageText.text = message.Content;

        TextMeshProUGUI timeSending = newMessage.transform.Find("TimeSending").GetComponent<TextMeshProUGUI>();
        timeSending.text = message.TimeSending.ToString();

        await TaskRunner.RunWithGlobalErrorHandling(async () =>
        {
            var request = new LoadImageRequest(
                message.FrameFileName,
                message.FrameS3Path);
            var frame = await _imagesService.LoadImage(request);

            Image playerFrame = newMessage.transform.Find("Frame").GetComponent<Image>();
            playerFrame.sprite = frame;
        });

        await TaskRunner.RunWithGlobalErrorHandling(async () =>
        {
            var request = new LoadImageRequest(
                message.AvatarFileName,
                message.AvatarS3Path);
            var avatar = await _imagesService.LoadImage(request);

            Image playerAvatar = newMessage.transform.Find("Frame/Avatar").GetComponent<Image>();
            playerAvatar.sprite = avatar;
        });

        Canvas.ForceUpdateCanvases();
        _scrollRect.verticalNormalizedPosition = 0f;
    }
}