using Project.Core.Entities.Business.DTOs.MessageDTOs;
using Project.Core.Entities.General;
using Project.Core.Interfaces.IRepositories;

namespace Project.API.SignalR.Service {
    public class MessageService : IMessageService {
        private readonly IMessageRepository _messageRepository;
        private readonly IProfileRepository _profileRepository;

        public MessageService(IMessageRepository messageRepository, IProfileRepository profileRepository) {
            _messageRepository = messageRepository;
            _profileRepository = profileRepository;
        }

        public async Task<MessageCreateResponse> SendMessage(MessageCreateRequest request) {
            var response = await _messageRepository.CreateMessage(request);
            return response;
        }
        public async Task<int> AmountNotRead(Guid userId) {
            return await _messageRepository.AmountNotRead(userId);
        }

        public async Task<Profile> GetUserProfile(string userEmail) {
            var user = await _profileRepository.GetProfileByEmail(userEmail);
            if (user != null) {
                return user;
            }
            else {
                return new Profile();
            }
        }
    }
}
