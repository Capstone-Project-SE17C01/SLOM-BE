using Project.Core.Entities.Business.DTOs.MessageDTOs;
using Project.Core.Entities.General;

namespace Project.API.SignalR.Service {
    public interface IMessageService {
        Task<MessageCreateResponse> SendMessage(MessageCreateRequest request);
        Task<int> AmountNotRead(Guid userId);
        Task<Profile> GetUserProfile(string userEmail);
    }
}
