namespace Project.Core.Entities.General {
    public class Word {
        public Guid Id { get; set; }

        public string? Text { get; set; }

        public string? VideoSrc { get; set; }

        public Guid? LessonId { get; set; }
        public Lesson? Lesson { get; set; }

        public ICollection<WordQuiz> WordQuizzes { get; set; } = new List<WordQuiz>();

    }
}
