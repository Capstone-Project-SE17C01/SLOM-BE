namespace Project.Infrastructure;

public partial class Translation {
    public long Id { get; set; }

    public Guid MeetingId { get; set; }

    public Guid? UserId { get; set; }

    public string? OriginalText { get; set; }

    public string TranslatedText { get; set; } = null!;

    public DateTime? Timestamp { get; set; }

    public Guid? SourceLanguage { get; set; }

    public Guid? TargetLanguage { get; set; }

    public virtual Meeting Meeting { get; set; } = null!;

    public virtual Language? SourceLanguageNavigation { get; set; }

    public virtual Language? TargetLanguageNavigation { get; set; }

    public virtual Profile? User { get; set; }
}
