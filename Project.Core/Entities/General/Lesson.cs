namespace Project.Infrastructure;

public partial class Lesson {
    public Guid Id { get; set; }

    public Guid ModuleId { get; set; }

    public string Title { get; set; } = null!;

    public string? Content { get; set; }

    public string? VideoUrl { get; set; }

    public int OrderNumber { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Module Module { get; set; } = null!;

    public virtual ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();

    public virtual ICollection<UserCourseProgress> UserCourseProgresses { get; set; } = new List<UserCourseProgress>();
}
