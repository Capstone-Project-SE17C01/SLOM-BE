namespace Project.Core.Entities.Business.DTOs.MeetingDTOs {
    public class AddRecordingDto {
        public required string StoragePath { get; set; }
        public int? Duration { get; set; }
        public required string UserId { get; set; }
    }
}
