using Project.Core.Entities.General;

namespace Project.Core.Interfaces.IServices {
    public interface IEmailService {
        Task<bool> SendMeetingScheduleEmailAsync(Meeting meeting, List<string> recipientEmails, string senderName, string? customMessage = null);
    }
}
