namespace Project.Core.Entities.General {
    public class UserCourseProgress {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid CourseId { get; set; }

        public DateTime? CompletedAt { get; set; }

        public Profile? User { get; set; }

        public Course? Course { get; set; }

        public bool IsActive { get; set; }

    }
}
