namespace Project.Infrastructure;

public partial class Course {
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? DifficultyLevel { get; set; }

    public string? ThumbnailUrl { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? LanguageId { get; set; }

    public Guid? CategoryId { get; set; }

    public virtual CourseCategory? Category { get; set; }

    public virtual ICollection<CourseReview> CourseReviews { get; set; } = new List<CourseReview>();

    public virtual Profile CreatedByNavigation { get; set; } = null!;

    public virtual Language? Language { get; set; }

    public virtual ICollection<Module> Modules { get; set; } = new List<Module>();
}
