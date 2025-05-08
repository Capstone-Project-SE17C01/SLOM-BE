namespace Project.Core.Entities.Business.DTOs.MessageDTOs {
    public class MessageCreateRequest {
        public required string SenderEmail { get; set; }
        public required string ReceiverEmail { get; set; }
        public required string Content { get; set; }
        public DateTime DateTime { get; set; } = DateTime.Now;
    }
}
