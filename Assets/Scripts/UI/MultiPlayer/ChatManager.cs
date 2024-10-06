using Assets.Scripts.Integrations.Firebase.Infrastructure.Requests;
using Assets.Scripts.Integrations.Firebase.Interfaces;
using System;
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

    private IFirebaseChatService _firebaseChatService;

    [Inject]
    public void Constructor(IFirebaseChatService firebaseChatService)
    {
        _firebaseChatService = firebaseChatService;
    }

    private void Start()
    {
        _firebaseChatService.SubscribeToNewMessages("Global", OnNewMessageReceived);
    }

    public void SendMessageToChat()
    {
        if (!string.IsNullOrEmpty(_input.text))
        {
            string nickname = PlayerPrefs.GetString("NickName");
            string imageUrl = PlayerPrefs.GetString("ImageUrl");

            var chatMessage = new SaveChatMessageRequest()
            {
                ChatId = "Global",
                Nickname = nickname,
                Message = _input.text,
                ImageUrl = imageUrl
            };

            try
            {
                _firebaseChatService.SendChatMessage(chatMessage);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }

            _input.text = string.Empty;
        }
    }

    private async void OnNewMessageReceived(SaveChatMessageRequest message)
    {
        GameObject newMessage = Instantiate(_messagePrefab, _chat);

        TextMeshProUGUI nicknameText = newMessage.transform.Find("NickName").GetComponent<TextMeshProUGUI>();
        nicknameText.text = message.Nickname;

        TextMeshProUGUI messageText = newMessage.transform.Find("Message/Text").GetComponent<TextMeshProUGUI>();
        messageText.text = message.Message;

        Image image = newMessage.transform.Find("PlayerPortret/PlayerImage").GetComponent<Image>();

        if (!ProfileManager.imageCache.TryGetValue(message.ImageUrl, out Sprite avatar))
        {
            avatar = await ProfileManager.Instance.GetImageFromUrl(message.ImageUrl);

            if (avatar != null)
            {
                ProfileManager.imageCache[message.ImageUrl] = avatar;
            }
        }

        image.sprite = avatar;

        Canvas.ForceUpdateCanvases();
        _scrollRect.verticalNormalizedPosition = 0f;
    }
}
