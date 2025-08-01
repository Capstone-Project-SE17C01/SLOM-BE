using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Project.Infrastructure.Data;

public class ReminderNotifier : BackgroundService {
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ReminderNotifier> _logger;

    public ReminderNotifier(IServiceProvider serviceProvider, ILogger<ReminderNotifier> logger) {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }
    private DateTime GetVietnamNow() {
        return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
            TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        while (!stoppingToken.IsCancellationRequested) {
            try {
                using (var scope = _serviceProvider.CreateScope()) {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var emailService = scope.ServiceProvider.GetRequiredService<Project.Core.Interfaces.IServices.IEmailService>();

                    var vnNow = GetVietnamNow();

                    var timeNow = TimeOnly.FromDateTime(vnNow);
                    var todayUtc = DateTime.UtcNow.Date;

                    var reminders = await dbContext.Reminders
                        .Where(r =>
                            r.IsActive &&
                            r.TimeToSend.Hour == timeNow.Hour &&
                            r.TimeToSend.Minute == timeNow.Minute &&
                            (r.LastSentDate == null || r.LastSentDate.Value.Date < todayUtc))
                        .ToListAsync();

                    foreach (var reminder in reminders) {
                        var senderName = "SLOM System";
                        var customMessage = "Reminder: Your study session is starting soon!";

                        var result = await emailService.SendCourseReminderEmailAsync(
                            reminder,
                            senderName,
                            customMessage
                        );

                        if (result) {
                            reminder.LastSentDate = DateTime.UtcNow;
                            dbContext.Reminders.Update(reminder);
                            await dbContext.SaveChangesAsync(stoppingToken);
                        }

                        _logger.LogInformation($"Sent course reminder for user {reminder.UserId}, result: {result}");
                    }
                }
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error in ScheduledMeetingNotifier");
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
