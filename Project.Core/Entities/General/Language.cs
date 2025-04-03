namespace Project.Infrastructure;

public partial class Language {
    public Guid Id { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Region { get; set; }

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

    public virtual ICollection<Profile> Profiles { get; set; } = new List<Profile>();

    public virtual ICollection<Translation> TranslationSourceLanguageNavigations { get; set; } = new List<Translation>();

    public virtual ICollection<Translation> TranslationTargetLanguageNavigations { get; set; } = new List<Translation>();
}
