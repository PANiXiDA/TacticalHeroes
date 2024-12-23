using System;


namespace Assets.Scripts.Infrastructure.Responses.ChatsService
{
    public class GlobalChatMessageResponse
    {
        public string MessageId { get; set; } = string.Empty;
        public int SenderId { get; set; }
        public string SenderNickname { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string AvatarS3Path { get; set; } = string.Empty;
        public string AvatarFileName { get; set; } = string.Empty;
        public string FrameS3Path { get; set; } = string.Empty;
        public string FrameFileName { get; set; } = string.Empty;
        public DateTime TimeSending { get; set; }
    }
}