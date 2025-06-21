using System.ComponentModel.DataAnnotations;

namespace Project.Core.Entities.Business.DTOs.MeetingDTOs {
    public class SendMeetingEmailDto {

        [Required]
        public List<string> RecipientEmails { get; set; } = new List<string>();

        public string? CustomMessage { get; set; }

        [Required]
        public string SenderName { get; set; } = string.Empty;
    }
}
