using Project.Core.Entities.Business.DTOs.MessageDTOs;
using Project.Core.Entities.General;
using Project.Core.Interfaces.IRepositories;

namespace Project.API.SignalR.Service {
    public class MessageService : IMessageService {
        private readonly IMessageRepository _messageRepository;

        public MessageService(IMessageRepository messageRepository, IProfileRepository profileRepository) {
            _messageRepository = messageRepository;
        }

        public async Task<MessageCreateResponse> SendMessage(MessageCreateRequest request) {
            var response = await _messageRepository.CreateMessage(request);
            return response;
        }
    }
}
