namespace Project.Core.Entities.Business.DTOs.WordDTOs {
    public class WordRequestDTO {
        public Guid? Id { get; set; }
        public string? Text { get; set; }
        public string? VideoSrc { get; set; }
        public Guid? LessonId { get; set; }
    }
}
