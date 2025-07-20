namespace Project.Core.Entities.Business.DTOs.QuizDTOs {
    public class QuizRequestDTO {
        public Guid Id { get; set; }

        public Guid LessonId { get; set; }

        public string Question { get; set; } = null!;

        public string CorrectAnswer { get; set; } = null!;

        public string? Explanation { get; set; }

        public int? MaxScore { get; set; }
    }
}
