namespace Project.Core.Entities.General {
    public class UserCourseProgress {
        public Guid UserId { get; set; }

        public Guid LessonId { get; set; }

        public DateTime CompletedAt { get; set; } = DateTime.UtcNow;

        public Profile User { get; set; } = null!;

        public Lesson Lesson { get; set; } = null!;
    }
}
