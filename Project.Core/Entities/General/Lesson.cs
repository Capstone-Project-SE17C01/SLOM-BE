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

        public Module Module { get; set; } = null!;
        public ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
        public ICollection<UserCourseProgress> UserProgresses { get; set; } = new List<UserCourseProgress>();
    }
}
