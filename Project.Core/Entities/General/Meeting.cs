namespace Project.Core.Entities.General {
    public class Meeting {
        public Guid Id { get; set; }

        public Guid HostId { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public string? Status { get; set; }

        public int MaxParticipants { get; set; } = 50;

        public bool IsPrivate { get; set; } = false;

        public string? GuestCode { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Profile Host { get; set; } = null!;
        public ICollection<MeetingParticipant> Participants { get; set; } = new List<MeetingParticipant>();
        public ICollection<MeetingRecording> Recordings { get; set; } = new List<MeetingRecording>();
        public ICollection<Translation> Translations { get; set; } = new List<Translation>();
    }
}
