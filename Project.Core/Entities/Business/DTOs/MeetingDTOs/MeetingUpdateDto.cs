namespace Project.Core.Entities.Business.DTOs.MeetingDTOs {
    public class MeetingUpdateDto {
        public required string UserId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string? Status { get; set; }
        public int? MaxParticipants { get; set; }
    }
}
