using System.ComponentModel.DataAnnotations;

namespace Project.Core.Entities.Business.DTOs.MeetingDTOs
{
    public class InvitationResponseDto
    {
        [Required]
        public string InvitationCode { get; set; } = string.Empty;
        
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        [Required]
        [RegularExpression("^(Accepted|Declined)$", ErrorMessage = "Response must be 'Accepted' or 'Declined'")]
        public string Response { get; set; } = string.Empty;
    }
}
