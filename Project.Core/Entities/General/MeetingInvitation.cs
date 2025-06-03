namespace Project.Core.Entities.General {
    public class MeetingInvitation {
        public Guid Id { get; set; }

        public Guid MeetingId { get; set; }

        public Guid? UserId { get; set; }

        public string? Email { get; set; }

        public string Status { get; set; } = "Pending"; // Pending, Accepted, Declined

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? RespondedAt { get; set; }

        public string? InvitationCode { get; set; }

        public Meeting Meeting { get; set; } = null!;
        public Profile? User { get; set; }
    }
}
