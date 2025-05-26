namespace Project.Core.Entities.Business.DTOs.MeetingDTOs {
    public class MeetingScheduleDto {
        public required string Title { get; set; }
        public string Description { get; set; } = string.Empty; 
        public required DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? Duration { get; set; }
        public bool IsPrivate { get; set; }
        public required string HostId { get; set; }
        public List<string> InvitedUserIds { get; set; } = new List<string>();
        public List<string> InvitedUserEmails { get; set; } = new List<string>();
        public int MaxParticipants { get; set; } = 50;
        public bool GenerateGuestCode { get; set; } = false;
    }
}
