using Project.Core.Entities.Business.DTOs.MessageDTOs;
using Project.Core.Entities.General;

namespace Project.Core.Interfaces.IRepositories {
    public interface IMessageRepository : IBaseRepository<UserMessage> {
        Task<MessageCreateResponse> CreateMessage(MessageCreateRequest request);
        Task<List<MessageUserResponse>> GetMessageUser(Guid userId);
        Task<MessageResponse> GetMessage(MessageRequest request);
        Task<bool> MarkIsRead(string senderEmail, string receiverEmail);
        Task<int> AmountNotRead(Guid userId);
    }
}
