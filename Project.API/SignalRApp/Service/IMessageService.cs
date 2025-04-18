using Project.Core.Entities.Business.DTOs.MessageDTOs;
using Project.Core.Entities.General;

namespace Project.API.SignalRApp.Service {
    public interface IMessageService {
        Task<MessageCreateResponse> SendMessage(MessageCreateRequest request);
    }
}
