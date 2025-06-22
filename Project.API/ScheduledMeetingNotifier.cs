using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;

public class ScheduledMeetingNotifier : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ScheduledMeetingNotifier> _logger;

    public ScheduledMeetingNotifier(IServiceProvider serviceProvider, ILogger<ScheduledMeetingNotifier> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var emailService = scope.ServiceProvider.GetRequiredService<Project.Core.Interfaces.IServices.IEmailService>();

                    var now = DateTime.UtcNow;
                    var today = now.Date;
                    var oneHourLater = now.AddHours(1);

                    var meetings = await dbContext.Meetings
                        .Include(m => m.Invitations)
                            .ThenInclude(p => p.User)
                        .Where(m =>
                            m.StartTime.Date == today &&
                            m.StartTime.Hour == oneHourLater.Hour &&
                            m.StartTime.Minute == oneHourLater.Minute
                        )
                        .ToListAsync(stoppingToken);

                    foreach (var meeting in meetings)
                    {
                        var recipientEmails = meeting.Invitations
                            .Select(p => p.Email)
                            .Where(email => !string.IsNullOrEmpty(email))
                            .Distinct()
                            .ToList();

                        if (recipientEmails.Count == 0)
                            continue;

                        var senderName = meeting.Host?.Username ?? "SLOM System";
                        var customMessage = "Reminder: Your meeting will start in 1 hour.";

                        var result = await emailService.SendMeetingScheduleEmailAsync(
                            meeting,
                            recipientEmails,
                            senderName,
                            customMessage
                        );

                        _logger.LogInformation($"Sent reminder for meeting {meeting.Id}, result: {result}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ScheduledMeetingNotifier");
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
