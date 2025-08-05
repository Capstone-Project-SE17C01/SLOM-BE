namespace Project.Core.Entities.Business.DTOs.QuestionDTOs {
    public class PostQuestionRequest {
        public Guid CreatorId { get; set; }
        public string Content { get; set; } = null!;
        public List<string>? Images { get; set; }
        public string Privacy { get; set; } = null!;
        public List<string>? Tags { get; set; }

    }
}
