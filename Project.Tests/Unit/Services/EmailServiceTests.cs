using Project.Core.Entities.General;
using Project.Infrastructure.Services;
using Project.Tests.Helpers;

namespace Project.Tests.Unit.Services {
    public class EmailServiceTests {
        private readonly EmailService _service;

        public EmailServiceTests() {
            var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>()).Build();
            _service = new EmailService(config);
        }

        [Test]
        public async Task SendMeetingScheduleEmailAsync_ShouldReturnTrue_WhenEmailSentSuccessfully() {
            var meeting = TestDataFixture.SingleMeeting;
            var recipients = new List<string> { "test@example.com" };

            var result = await _service.SendMeetingScheduleEmailAsync(meeting, recipients, "Sender");

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task SendMeetingScheduleEmailAsync_ShouldReturnFalse_WhenRecipientListIsEmpty() {
            var meeting = TestDataFixture.SingleMeeting;
            var recipients = new List<string>();

            var result = await _service.SendMeetingScheduleEmailAsync(meeting, recipients, "Sender");

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task SendCourseReminderEmailAsync_ShouldReturnTrue_WhenEmailSentSuccessfully() {
            var reminder = new Reminder();

            var result = await _service.SendCourseReminderEmailAsync(reminder, "Sender");

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task SendCourseReminderEmailAsync_ShouldReturnFalse_WhenReminderIsNull() {
            var result = await _service.SendCourseReminderEmailAsync(null, "Sender");

            Assert.That(result, Is.False);
        }
    }
}
