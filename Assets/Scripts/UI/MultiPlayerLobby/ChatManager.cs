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


    [Inject]
    public void Constructor()
    {
    }

    public void SendMessageToChat()
    {

    }

    private void OnNewMessageReceived()
    {
        GameObject newMessage = Instantiate(_messagePrefab, _chat);

        TextMeshProUGUI nicknameText = newMessage.transform.Find("NickName").GetComponent<TextMeshProUGUI>();
        nicknameText.text = string.Empty;

        TextMeshProUGUI messageText = newMessage.transform.Find("Message/Text").GetComponent<TextMeshProUGUI>();
        messageText.text = string.Empty;

        Image image = newMessage.transform.Find("PlayerPortret/PlayerImage").GetComponent<Image>();

        //if (!ProfileManager.imageCache.TryGetValue(message.ImageUrl, out Sprite avatar))
        //{
        //    avatar = await ProfileManager.Instance.GetImageFromUrl(message.ImageUrl);

        //    if (avatar != null)
        //    {
        //        ProfileManager.imageCache[message.ImageUrl] = avatar;
        //    }
        //}

        //image.sprite = avatar;

        Canvas.ForceUpdateCanvases();
        _scrollRect.verticalNormalizedPosition = 0f;
    }
}
