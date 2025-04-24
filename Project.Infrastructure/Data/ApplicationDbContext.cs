using Microsoft.EntityFrameworkCore;
using Project.Core.Entities.General;

namespace Project.Infrastructure.Data;

public partial class ApplicationDbContext : DbContext {
    public ApplicationDbContext() {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) {
    }

    public DbSet<Language> Languages { get; set; }
    public DbSet<Profile> Profiles { get; set; }
    public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
    public DbSet<UserSubscription> UserSubscriptions { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Meeting> Meetings { get; set; }
    public DbSet<MeetingParticipant> MeetingParticipants { get; set; }
    public DbSet<MeetingRecording> MeetingRecordings { get; set; }
    public DbSet<Translation> Translations { get; set; }
    public DbSet<CourseCategory> CourseCategories { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<Module> Modules { get; set; }
    public DbSet<Lesson> Lessons { get; set; }
    public DbSet<Quiz> Quizzes { get; set; }
    public DbSet<QuizAttempt> QuizAttempts { get; set; }
    public DbSet<UserCourseProgress> UserCourseProgress { get; set; }
    public DbSet<CourseReview> CourseReviews { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserMessage> UserMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder builder) {
        ApplicationDbContextConfigurations.Configure(builder);
        ApplicationDbContextConfigurations.SeedData(builder);
    }
}
