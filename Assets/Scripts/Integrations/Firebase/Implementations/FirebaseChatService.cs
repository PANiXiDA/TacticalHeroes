using Assets.Scripts.Integrations.Firebase.Infrastructure.Requests;
using Assets.Scripts.Integrations.Firebase.Interfaces;
using Firebase.Database;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Integrations.Firebase.Implementations
{
    internal class FirebaseChatService : IFirebaseChatService
    {
        private DatabaseReference db;

        public FirebaseChatService()
        {
            db = FirebaseDatabase.DefaultInstance.RootReference;
        }

        public void SendChatMessage(SaveChatMessageRequest message)
        {
            string key = db.Child("Chats").Child(message.ChatId).Child("Messages").Push().Key;

            var messageData = new Dictionary<string, object>()
            {
                { "Nickname", message.Nickname },
                { "Message", message.Message },
                { "ImageUrl", message.ImageUrl },
                { "CreatedAt", ServerValue.Timestamp }
            };

            db.Child("Chats").Child(message.ChatId).Child("Messages").Child(key).SetValueAsync(messageData);
        }

        public void SubscribeToNewMessages(string chatId, Action<SaveChatMessageRequest> onNewMessage)
        {
            long currentTimeMillis = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            long twoHoursAgoMillis = (long)(currentTimeMillis - TimeSpan.FromHours(2).TotalMilliseconds);

            DatabaseReference chatRef = db.Child("Chats").Child(chatId).Child("Messages");

            chatRef.OrderByChild("CreatedAt").StartAt(twoHoursAgoMillis).ChildAdded += (object sender, ChildChangedEventArgs args) =>
            {
                if (args.Snapshot != null && args.Snapshot.Exists)
                {
                    string nickname = args.Snapshot.Child("Nickname").Value.ToString();
                    string message = args.Snapshot.Child("Message").Value.ToString();
                    string imageUrl = args.Snapshot.Child("ImageUrl").Value.ToString();

                    var chatMessage = new SaveChatMessageRequest
                    {
                        ChatId = chatId,
                        Nickname = nickname,
                        Message = message,
                        ImageUrl = imageUrl
                    };

                    onNewMessage?.Invoke(chatMessage);
                }
            };
        }
    }
}
