namespace Project.Infrastructure;

public partial class CourseCategory {
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public Guid? ParentCategoryId { get; set; }

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

    public virtual ICollection<CourseCategory> InverseParentCategory { get; set; } = new List<CourseCategory>();

    public virtual CourseCategory? ParentCategory { get; set; }
}
