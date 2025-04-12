namespace Project.Core.Entities.General {
    public class Translation {
        public long Id { get; set; }

        public Guid? MeetingId { get; set; }

        public Guid? UserId { get; set; }

        public Guid? SourceLanguageId { get; set; }

        public Guid? TargetLanguageId { get; set; }

        public string? OriginalText { get; set; }

        public string TranslatedText { get; set; } = null!;

        public float? Confidence { get; set; }

        public bool IsCorrected { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Meeting Meeting { get; set; } = null!;
        public Profile? User { get; set; }
        public Language? SourceLanguage { get; set; }
        public Language? TargetLanguage { get; set; }
    }
}
