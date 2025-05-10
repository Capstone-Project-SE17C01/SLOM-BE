namespace Project.Core.Entities.General {
    public class Course {
        public Guid Id { get; set; }

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public string? DifficultyLevel { get; set; }

        public string? ThumbnailUrl { get; set; }

        public Guid? LanguageId { get; set; }

        public Guid? CategoryId { get; set; }

        public Guid? CreatorId { get; set; }

        public bool IsPublished { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public Language? Language { get; set; }
        public CourseCategory? Category { get; set; }
        public Profile Creator { get; set; } = null!;
        public ICollection<Module> Modules { get; set; } = new List<Module>();
        public ICollection<CourseReview> Reviews { get; set; } = new List<CourseReview>();
    }
}
