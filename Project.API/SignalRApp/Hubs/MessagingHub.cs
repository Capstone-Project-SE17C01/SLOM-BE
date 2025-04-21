using Microsoft.AspNetCore.SignalR;
using Project.API.SignalRApp.Service;
using Project.Core.Entities.Business.DTOs.MessageDTOs;
using Project.Core.Entities.General;
using Project.Core.Interfaces.IRepositories;

namespace Project.API.SignalRApp.Hubs {
    public class MessagingHub : Hub {
        private readonly IMessageService _messageService;

        private static readonly List<UserMessage> MessageHistory = new List<UserMessage>();

        public MessagingHub(IMessageService messageService) {
            _messageService = messageService;
        }

        public async Task PostMessage(string content, string senderName, string receiverName) {
            var userMessage = new MessageCreateRequest {
                Content = content,
                ReceiverUserName = receiverName,
                SenderUserName = senderName,
                DateTime = DateTime.UtcNow
            };

            var message = await _messageService.SendMessage(userMessage);

            await Clients.Others.SendAsync("ReceiveMessage" + receiverName, message.MessageId, senderName, content, userMessage.DateTime);
        }

        public async Task RetrieveMessageHistory() =>
            await Clients.Caller.SendAsync("MessageHistory", MessageHistory);
    }
}
