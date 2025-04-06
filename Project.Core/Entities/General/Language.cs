namespace Project.Core.Entities.General {
    public class Language {
        public Guid Id { get; set; }
        public string Code { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string? Region { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Profile> Profiles { get; set; } = new List<Profile>();
        public ICollection<Course> Courses { get; set; } = new List<Course>();
        public ICollection<Translation> SourceTranslations { get; set; } = new List<Translation>();
        public ICollection<Translation> TargetTranslations { get; set; } = new List<Translation>();
    }
}
