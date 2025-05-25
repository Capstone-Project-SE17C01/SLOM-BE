using System.ComponentModel.DataAnnotations;

namespace Project.Core.Entities.Business.DTOs.MeetingDTOs
{
    public class MeetingInviteDto
    {
        [Required]
        public string HostId { get; set; } = string.Empty;
        
        [Required]
        public Guid MeetingId { get; set; }
        
        public List<Guid>? UserIds { get; set; }
        
        public List<string>? Emails { get; set; }
        
        public string? Message { get; set; }
    }
}
