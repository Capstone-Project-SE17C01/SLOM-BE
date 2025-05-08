namespace Project.Core.Entities.Business.DTOs.MessageDTOs {
    public class MessageContent {
        public int Id { get; set; }
        public required string Content { get; set; }
        public bool IsSender { get; set; }
    }

    public class MessageResponse {
        public bool IsLoadFullPage { get; set; }
        public required List<MessageContent> Data { get; set; }
    }
}
