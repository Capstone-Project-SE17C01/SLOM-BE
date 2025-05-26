namespace Project.Core.Entities.Business.DTOs.MeetingDTOs {
    public class MeetingCreateDto {
        public required string Title { get; set; }
        public string Description { get; set; } = string.Empty; 
        public bool IsImmediate { get; set; }
        public DateTime? StartTime { get; set; }
        public int? Duration { get; set; }
        public bool IsPrivate { get; set; }
        public required string UserId { get; set; }
    }
}
