namespace Project.Core.Entities.General {
    public class UserLessonProgress {
        public Guid UserId { get; set; }

        public Guid LessonId { get; set; }

        public DateTime? CompletedAt { get; set; }

        public Profile? User { get; set; }

        public Lesson? Lesson { get; set; }

        public bool IsCompleted {
            get {
                return CompletedAt != null;
            }
        }

        public bool IsActive { get; set; }
    }
}
