namespace Project.Core.Entities.Business.DTOs.MessageDTOs {
    public class MessageCreateResponse {
        public required string Sender { get; set; }
        public required string Message { get; set; }
        public int MessageId { get; set; }
    }
}
