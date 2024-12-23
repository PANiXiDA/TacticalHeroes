namespace Assets.Scripts.Infrastructure.Requests.ChatsService
{
    public class GlobalChatMessageRequest
    {
        public string Content { get; set; }

        public GlobalChatMessageRequest(string content)
        {
            Content = content;
        }
    }
}
