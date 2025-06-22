using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Project.Core.Entities.General;
using Project.Core.Interfaces.IServices;

namespace Project.Infrastructure.Services {
    public class EmailService : IEmailService {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration) {
            _configuration = configuration;
        }

        public async Task<bool> SendMeetingScheduleEmailAsync(Meeting meeting, List<string> recipientEmails, string senderName, string? customMessage = null) {
            try {
                var smtpHost = _configuration["EmailSettings:SmtpHost"] ?? "smtp.gmail.com";
                var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
                var smtpEmail = _configuration["EmailSettings:SmtpEmail"];
                var smtpPassword = _configuration["EmailSettings:SmtpPassword"];
                var frontendUrl = _configuration["EmailSettings:FrontendUrl"] ?? "http://localhost:3000";

                if (string.IsNullOrEmpty(smtpEmail) || string.IsNullOrEmpty(smtpPassword)) {
                    throw new InvalidOperationException("Email configuration is missing");
                }

                using var client = new SmtpClient(smtpHost, smtpPort) {
                    Credentials = new NetworkCredential(smtpEmail, smtpPassword),
                    EnableSsl = true
                };

                var meetingLink = $"{frontendUrl}/meeting?roomID={meeting.Id}";
                var subject = $"Meeting Invitation: {meeting.Title}";
                var htmlBody = GenerateEmailTemplate(meeting, meetingLink, senderName, customMessage);

                foreach (var email in recipientEmails) {
                    var message = new MailMessage {
                        From = new MailAddress(smtpEmail, "SLOM Meeting System"),
                        Subject = subject,
                        Body = htmlBody,
                        IsBodyHtml = true
                    };

                    message.To.Add(email);
                    await client.SendMailAsync(message);
                }

                return true;
            }
            catch (Exception ex) {
                Console.WriteLine($"Error sending email: {ex.Message}");
                return false;
            }
        }

        private string GenerateEmailTemplate(Meeting meeting, string meetingLink, string senderName, string? customMessage) {
            var templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "EmailTemplate.html");
            string template = File.ReadAllText(templatePath);

            var startTime = meeting.StartTime.ToString("dddd, MMMM dd, yyyy 'at' HH:mm");
            var endTime = meeting.EndTime?.ToString("HH:mm") ?? "TBD";

            template = template.Replace("{{SenderName}}", senderName);
            template = template.Replace("{{MeetingTitle}}", meeting.Title);
            template = template.Replace("{{StartTime}}", startTime);
            template = template.Replace("{{EndTime}}", endTime);
            template = template.Replace("{{MeetingLink}}", meetingLink);
            template = template.Replace("{{MeetingType}}", meeting.IsPrivate ? "Private Meeting" : "Public Meeting");

            if (!string.IsNullOrEmpty(meeting.Description)) {
                var descriptionHtml = $@"
            <div class='detail-row'>
                <span class='detail-label'>📝 Description:</span>
                <span class='detail-value'>{meeting.Description}</span>
            </div>";
                template = template.Replace("{{MeetingDescription}}", descriptionHtml);
            }
            else {
                template = template.Replace("{{MeetingDescription}}", "");
            }

            if (meeting.IsPrivate && !string.IsNullOrEmpty(meeting.GuestCode)) {
                var guestCodeHtml = $@"
            <div class='detail-row'>
                <span class='detail-label'>🔑 Guest Code:</span>
                <span class='detail-value'><strong>{meeting.GuestCode}</strong></span>
            </div>";
                template = template.Replace("{{GuestCodeSection}}", guestCodeHtml);
            }
            else {
                template = template.Replace("{{GuestCodeSection}}", "");
            }

            if (!string.IsNullOrEmpty(customMessage)) {
                var customMessageHtml = $@"
        <div class='custom-message'>
            <h3>📨 Personal Message:</h3>
            <p>{customMessage}</p>
        </div>";
                template = template.Replace("{{CustomMessageSection}}", customMessageHtml);
            }
            else {
                template = template.Replace("{{CustomMessageSection}}", "");
            }

            return template;
        }

        public async Task<bool> SendCourseReminderEmailAsync(Reminder reminder, string senderName, string? customMessage = null) {
            try {
                var smtpHost = _configuration["EmailSettings:SmtpHost"] ?? "smtp.gmail.com";
                var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
                var smtpEmail = _configuration["EmailSettings:SmtpEmail"];
                var smtpPassword = _configuration["EmailSettings:SmtpPassword"];
                var frontendUrl = _configuration["EmailSettings:FrontendUrl"] ?? "http://localhost:3000";

                if (string.IsNullOrEmpty(smtpEmail) || string.IsNullOrEmpty(smtpPassword)) {
                    throw new InvalidOperationException("Email configuration is missing");
                }

                using var client = new SmtpClient(smtpHost, smtpPort) {
                    Credentials = new NetworkCredential(smtpEmail, smtpPassword),
                    EnableSsl = true
                };

                var subject = "⏰ It's time to study! - SLOM Reminder";
                var htmlBody = GenerateReminderEmailTemplate(reminder, senderName, customMessage);
                var message = new MailMessage {
                    From = new MailAddress(smtpEmail, "SLOM Meeting System"),
                    Subject = subject,
                    Body = htmlBody,
                    IsBodyHtml = true
                };

                message.To.Add(reminder.Email);
                await client.SendMailAsync(message);

                return true;
            }
            catch (Exception ex) {
                Console.WriteLine($"Error sending email: {ex.Message}");
                return false;
            }
        }

        private string GenerateReminderEmailTemplate(Reminder reminder, string senderName, string? customMessage) {
            var templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "EmailTemplateRemind.html");
            string template = File.ReadAllText(templatePath);

            var remindTime = reminder.TimeToSend.ToString(@"hh\:mm");

            template = template.Replace("{{SenderName}}", senderName);
            template = template.Replace("{{RemindTitle}}", "⏰ Remind Learning - SLOM Reminder");
            template = template.Replace("{{RemindTime}}", remindTime);

            if (!string.IsNullOrEmpty(reminder.Message)) {
                var descriptionHtml = $@"
            <div class='detail-row'>
                <span class='detail-label'>📚 Desciption:</span>
                <span class='detail-value'>{reminder.Message}</span>
            </div>";
                template = template.Replace("{{RemindDescription}}", descriptionHtml);
            }
            else {
                template = template.Replace("{{RemindDescription}}", "");
            }


            if (!string.IsNullOrEmpty(customMessage)) {
                var customMessageHtml = $@"
            <div class='custom-message'>
                <h3>📨 Personal Message:</h3>
                <p>{customMessage}</p>
            </div>";
                template = template.Replace("{{CustomMessageSection}}", customMessageHtml);
            }
            else {
                template = template.Replace("{{CustomMessageSection}}", "");
            }

            return template;
        }
    }
}
