namespace Project.Core.Entities.General {
    public class Quiz {
        public Guid Id { get; set; }

        public Guid LessonId { get; set; }

        public string Question { get; set; } = null!;

        public string? Options { get; set; }

        public string CorrectAnswer { get; set; } = null!;

        public string? Explanation { get; set; }

        public int? MaxScore { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Lesson Lesson { get; set; } = null!;

        public ICollection<QuizAttempt> Attempts { get; set; } = new List<QuizAttempt>();
    }
}
