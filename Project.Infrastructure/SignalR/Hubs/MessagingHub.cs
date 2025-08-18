using Microsoft.AspNetCore.SignalR;
using Project.API.SignalR.Service;
using Project.Core.Entities.Business.DTOs.MessageDTOs;
using Project.Core.Entities.General;
using Project.Core.Interfaces.IRepositories;

namespace Project.API.SignalR.Hubs {
    public class MessagingHub : Hub {
        private readonly IMessageService _messageService;

        private static readonly List<UserMessage> MessageHistory = new List<UserMessage>();

        public MessagingHub(IMessageService messageService, IProfileRepository profileRepository) {
            _messageService = messageService;
        }

        public async Task PostMessage(string content, string senderEmail, string receiverEmail, List<string> images, string image) {
            var userMessage = new MessageCreateRequest {
                Content = content,
                ReceiverEmail = receiverEmail,
                SenderEmail = senderEmail,
                DateTime = DateTime.UtcNow,
                Images = images
            };

            var message = await _messageService.SendMessage(userMessage);
            var sender = await _messageService.GetUserProfile(senderEmail);
            var receiver = await _messageService.GetUserProfile(receiverEmail);

            await Clients.Others.SendAsync("ReceiveMessage" + receiverEmail, message.MessageId, senderEmail, content, GetTimeAgo(userMessage.DateTime), images, image, sender.Username);

            var isAdd = await _messageService.AmountNotRead(receiver.Id);
            await Clients.Others.SendAsync(receiverEmail, isAdd);
        }

        public async Task RetrieveMessageHistory() =>
            await Clients.Caller.SendAsync("MessageHistory", MessageHistory);

        private string GetTimeAgo(DateTime messageTime) {
            var timeSpan = DateTime.UtcNow - messageTime;

            if (timeSpan.TotalSeconds < 60)
                return $"{(int)timeSpan.TotalSeconds} seconds";
            if (timeSpan.TotalMinutes < 60)
                return $"{(int)timeSpan.TotalMinutes} minutes";
            if (timeSpan.TotalHours < 24)
                return $"{(int)timeSpan.TotalHours} hours";
            if (timeSpan.TotalDays < 7)
                return $"{(int)timeSpan.TotalDays} days";
            if (timeSpan.TotalDays < 30)
                return $"{(int)(timeSpan.TotalDays / 7)} weeks";
            if (timeSpan.TotalDays < 365)
                return $"{(int)(timeSpan.TotalDays / 30)} months";

            return $"{(int)(timeSpan.TotalDays / 365)} years";
        }
    }
}
