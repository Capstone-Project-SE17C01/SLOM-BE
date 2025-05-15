namespace Project.Core.Entities.General {
    public class QuizOption {
        public Guid Id { get; set; }

        public string? Text { get; set; }

        public Guid? QuizId { get; set; }

        public Quiz? Quiz { get; set; } = null!;

        public bool IsCorrect { get; set; } = false;

    }
}
