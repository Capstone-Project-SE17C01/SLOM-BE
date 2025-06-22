namespace Project.Core.Entities.Business.DTOs.MeetingDTOs
{
    public class MeetingInvitationDto
    {
        public Guid MeetingId { get; set; }
        public List<string> Email { get; set; } = new List<string>();
    }
}
