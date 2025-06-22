namespace Project.Core.Entities.General {
    public class Reminder {
        public Guid Id { get; set; }

        public string Email { get; set; } = null!;

        public string? Message { get; set; }

        public TimeOnly TimeToSend { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastSentDate { get; set; }

        public Guid? UserId { get; set; }

        public Profile? User { get; set; }

    }

}
