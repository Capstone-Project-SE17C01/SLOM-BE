namespace Project.Core.Entities.General {
    public class CourseReview {
        public Guid Id { get; set; }

        public Guid CourseId { get; set; }

        public Guid UserId { get; set; }

        public int Rating { get; set; }

        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Course Course { get; set; } = null!;
        public Profile User { get; set; } = null!;
    }
}
