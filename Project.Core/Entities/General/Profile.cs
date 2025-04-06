namespace Project.Core.Entities.General {
    public class Profile {
        public Guid Id { get; set; }

        public string? Username { get; set; }

        public string? Email { get; set; }

        public Guid? RoleId { get; set; }

        public string? AvatarUrl { get; set; }

        public Guid? PreferredLanguageId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public Language? PreferredLanguage { get; set; }
        public ICollection<UserSubscription> UserSubscriptions { get; set; } = new List<UserSubscription>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public ICollection<Meeting> HostedMeetings { get; set; } = new List<Meeting>();
        public ICollection<MeetingParticipant> MeetingParticipations { get; set; } = new List<MeetingParticipant>();
        public ICollection<Translation> Translations { get; set; } = new List<Translation>();
        public ICollection<Course> CreatedCourses { get; set; } = new List<Course>();
        public ICollection<QuizAttempt> QuizAttempts { get; set; } = new List<QuizAttempt>();
        public ICollection<UserCourseProgress> CourseProgresses { get; set; } = new List<UserCourseProgress>();
        public ICollection<CourseReview> CourseReviews { get; set; } = new List<CourseReview>();
        public Role? Role { get; set; }
    }
}
