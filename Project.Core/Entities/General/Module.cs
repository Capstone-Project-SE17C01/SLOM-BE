namespace Project.Core.Entities.General {
    public class Module {
        public Guid Id { get; set; }

        public Guid CourseId { get; set; }

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public int OrderNumber { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Course? Course { get; set; }
        public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
        public ICollection<UserModuleProgress> UserModuleProgress { get; set; } = new List<UserModuleProgress>();
    }
}
