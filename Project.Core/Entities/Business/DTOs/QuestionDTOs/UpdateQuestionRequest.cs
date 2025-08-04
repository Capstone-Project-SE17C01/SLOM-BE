namespace Project.Core.Entities.Business.DTOs.QuestionDTOs {
    public class UpdateQuestionRequest {
        public Guid QuestionId { get; set; }
        public string Content { get; set; } = null!;
        public List<string>? Images { get; set; }
        public string Privacy { get; set; } = null!;
    }
}
