namespace Project.Core.Entities.Business.DTOs.MessageDTOs {
    public class MessageRequest {
        public Guid UserId { get; set; }
        public required string ReceiverEmail { get; set; }
        public int PageNumber { get; set; }
    }
}
