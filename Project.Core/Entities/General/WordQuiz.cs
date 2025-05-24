namespace Project.Core.Entities.General {
    public class WordQuiz {
        public Guid Id { get; set; }

        public string? Text { get; set; }

        public string? VideoSrc { get; set; }

        public Guid? WordId { get; set; }

        public Word? Word { get; set; } = null!;

        public Guid? QuizId { get; set; }

        public Quiz? Quiz { get; set; } = null!;
    }
}
