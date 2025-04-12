namespace Project.Core.Entities.General {
    public class MeetingRecording {
        public Guid Id { get; set; }

        public Guid MeetingId { get; set; }

        public string StoragePath { get; set; } = null!;

        public int? Duration { get; set; }

        public bool Processed { get; set; } = false;

        public string? Transcription { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Meeting Meeting { get; set; } = null!;
    }
}
