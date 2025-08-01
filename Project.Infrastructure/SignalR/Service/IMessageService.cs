using Project.Core.Entities.Business.DTOs.MessageDTOs;

namespace Project.API.SignalR.Service {
    public interface IMessageService {
        Task<MessageCreateResponse> SendMessage(MessageCreateRequest request);
    }
}
