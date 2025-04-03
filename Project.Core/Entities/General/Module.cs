namespace Project.Infrastructure;

public partial class Module {
    public Guid Id { get; set; }

    public Guid CourseId { get; set; }

    public string Title { get; set; } = null!;

    public int OrderNumber { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}
