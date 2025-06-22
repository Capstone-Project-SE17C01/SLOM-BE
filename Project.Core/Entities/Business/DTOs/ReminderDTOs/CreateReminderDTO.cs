namespace Project.Core.Entities.Business.DTOs.ReminderDTOs {
    public class CreateReminderDTO {
        public string Email { get; set; } = null!;
        public string? Message { get; set; }
        public Guid? UserId { get; set; }
        public TimeOnly TimeToSend { get; set; }
        public bool IsActive { get; set; }
    }
}
