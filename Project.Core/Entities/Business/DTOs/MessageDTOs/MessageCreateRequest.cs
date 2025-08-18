namespace Project.Core.Entities.Business.DTOs.MessageDTOs {
    public class MessageCreateRequest {
        public required string SenderEmail { get; set; }
        public required string ReceiverEmail { get; set; }
        public required string Content { get; set; }
        public DateTime DateTime { get; set; } = DateTime.Now;
        public List<string> Images { get; set; } = null!;
        public bool? IsRead { get; set; }
    }
}
