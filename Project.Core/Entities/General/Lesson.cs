namespace Project.Core.Entities.General {
    public class Lesson {
        public Guid Id { get; set; }
        public Guid ModuleId { get; set; }

        public string Title { get; set; } = null!;

        public string? Content { get; set; }

        public string? VideoUrl { get; set; }

        public int? DurationMinutes { get; set; }

        public int OrderNumber { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Module? Module { get; set; }
        public ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
        public ICollection<Word> Words { get; set; } = new List<Word>();
        public ICollection<UserLessonProgress> UserLessonProgress { get; set; } = new List<UserLessonProgress>();
    }
}
