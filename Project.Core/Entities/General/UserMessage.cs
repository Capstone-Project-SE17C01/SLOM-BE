namespace Project.Core.Entities.General {
    public class UserMessage {
        public int MessageId { get; set; }
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public required string Message { get; set; }
        public DateTime DateTime { get; set; }
        public required Profile Sender { get; set; }
        public required Profile Receiver { get; set; }
    }
}
