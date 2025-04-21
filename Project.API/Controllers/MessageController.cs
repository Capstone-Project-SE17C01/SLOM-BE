using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.Core.Entities.Business.DTOs.MessageDTOs;
using Project.Core.Entities.Business.DTOs.Profile;
using Project.Core.Entities.General;
using Project.Core.Interfaces.IRepositories;

namespace Project.API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase {
        private readonly IMessageRepository _messageRepository;

        public MessageController(IMessageRepository messageRepository) {
            _messageRepository = messageRepository;
        }

        [HttpGet("GetUserMessage")]
        public async Task<List<MessageUserResponse>> GetUserMessage(Guid UserId) {
            return await _messageRepository.GetMessageUser(UserId);
        }

        [HttpGet("GetMessage")]
        public async Task<MessageResponse> GetMessage(string userId, string receiverName, int pageNumber) {
            return await _messageRepository.GetMessage(new MessageRequest { UserId = Guid.Parse(userId), PageNumber = pageNumber, ReceiverName = receiverName});
        } 
    }
}
