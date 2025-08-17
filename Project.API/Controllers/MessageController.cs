using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.Core.Entities.Business.DTOs.MessageDTOs;
using Project.Core.Interfaces.IRepositories;

namespace Project.API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
        public async Task<MessageResponse> GetMessage(string userId, string receiverEmail, int pageNumber) {
            return await _messageRepository.GetMessage(new MessageRequest { UserId = Guid.Parse(userId), PageNumber = pageNumber, ReceiverEmail = receiverEmail });
        }

        [HttpPut("MarkIsRead")]
        public async Task<IActionResult> MarkIsRead(string senderEmail, string receiverEmail) {
            try {
                var result = await _messageRepository.MarkIsRead(senderEmail, receiverEmail);
                if (result) {
                    return Ok(new { success = true, message = "Messages marked as read successfully" });
                }
                return Ok(new { success = false, message = "No unread messages found" });
            }
            catch (Exception ex) {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("GetMessageNotRead")]
        public async Task<IActionResult> GetMessageNotRead(string userId) {
            try {
                var result = await _messageRepository.AmountNotRead(Guid.Parse(userId));
                return Ok(new { success = true, result = result });
            } catch (Exception ex) {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
