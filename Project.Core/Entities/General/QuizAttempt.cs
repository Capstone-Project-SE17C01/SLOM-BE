namespace Project.Core.Entities.General {
    public class QuizAttempt {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid QuizId { get; set; }

        public string? SelectedAnswer { get; set; }

        public int Score { get; set; }

        public DateTime CompletedAt { get; set; } = DateTime.UtcNow;

        public Profile User { get; set; } = null!;
        public Quiz Quiz { get; set; } = null!;
    }
}
