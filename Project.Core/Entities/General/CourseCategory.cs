namespace Project.Core.Entities.General {
    public class CourseCategory {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public Guid? ParentId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public CourseCategory? Parent { get; set; }
        public ICollection<CourseCategory> Subcategories { get; set; } = new List<CourseCategory>();
        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
