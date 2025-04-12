namespace Project.Core.Entities.General {
    public class MeetingParticipant {
        public Guid MeetingId { get; set; }

        public Guid UserId { get; set; }

        public DateTime JoinTime { get; set; } = DateTime.UtcNow;

        public DateTime? LeaveTime { get; set; }

        public string? DeviceInfo { get; set; }

        public Meeting Meeting { get; set; } = null!;
        public Profile User { get; set; } = null!;
    }
}
