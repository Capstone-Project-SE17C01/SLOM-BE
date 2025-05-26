using System.ComponentModel.DataAnnotations;

namespace Project.Core.Entities.Business.DTOs.MeetingDTOs
{
    public class MeetingScheduleWithInvitesDto
    {
        [Required]
        public string HostId { get; set; } = string.Empty;
        
        [Required]
        public string Title { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        [Required]
        public DateTime StartTime { get; set; }
        
        public DateTime? EndTime { get; set; }
        
        public int? Duration { get; set; }
        
        public bool IsPrivate { get; set; } = false;
        
        public int MaxParticipants { get; set; } = 50;
        
        public List<Guid>? InviteUserIds { get; set; }
        
        public List<string>? InviteEmails { get; set; }
        
        public string? InvitationMessage { get; set; }
    }
}
