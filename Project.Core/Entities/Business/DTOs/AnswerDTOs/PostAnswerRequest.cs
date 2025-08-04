namespace Project.Core.Entities.Business.DTOs.AnswerDTOs {
    public class PostAnswerRequest {
        public Guid CreatorId { get; set; }
        public Guid QuestionId { get; set; }
        public string Content { get; set; } = null!;
        public List<string>? Images { get; set; } = null!;
    }
}
