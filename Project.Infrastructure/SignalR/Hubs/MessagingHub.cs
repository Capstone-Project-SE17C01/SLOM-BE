using Microsoft.AspNetCore.SignalR;
using Project.API.SignalR.Service;
using Project.Core.Entities.Business.DTOs.MessageDTOs;
using Project.Core.Entities.General;

namespace Project.API.SignalR.Hubs {
    public class MessagingHub : Hub {
        private readonly IMessageService _messageService;

        private static readonly List<UserMessage> MessageHistory = new List<UserMessage>();

        public MessagingHub(IMessageService messageService) {
            _messageService = messageService;
        }

        public async Task PostMessage(string content, string senderEmail, string receiverEmail) {
            var userMessage = new MessageCreateRequest {
                Content = content,
                ReceiverEmail = receiverEmail,
                SenderEmail = senderEmail,
                DateTime = DateTime.UtcNow
            };

            var message = await _messageService.SendMessage(userMessage);

            await Clients.Others.SendAsync("ReceiveMessage" + receiverEmail, message.MessageId, senderEmail, content, userMessage.DateTime);
        }

        public async Task RetrieveMessageHistory() =>
            await Clients.Caller.SendAsync("MessageHistory", MessageHistory);
    }
}
