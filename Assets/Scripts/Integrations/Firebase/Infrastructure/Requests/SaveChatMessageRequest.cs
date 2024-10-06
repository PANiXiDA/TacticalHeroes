namespace Assets.Scripts.Integrations.Firebase.Infrastructure.Requests
{
    public class SaveChatMessageRequest
    {
        public string ChatId { get; set; }
        public string Nickname { get; set; }
        public string Message { get; set; }
        public string ImageUrl { get; set; }
    }
}
