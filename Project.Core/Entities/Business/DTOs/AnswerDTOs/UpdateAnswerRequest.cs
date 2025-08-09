namespace Project.Core.Entities.Business.DTOs.AnswerDTOs {
    public class UpdateAnswerRequest {
        public Guid AnswerId { get; set; }
        public string Content { get; set; } = null!;
        public List<string>? Images { get; set; } = null!;
    }
}
