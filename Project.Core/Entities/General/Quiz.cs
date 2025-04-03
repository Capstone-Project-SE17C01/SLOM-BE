namespace Project.Infrastructure;

public partial class Quiz {
    public Guid Id { get; set; }

    public Guid LessonId { get; set; }

    public string Question { get; set; } = null!;

    public string? Options { get; set; }

    public string CorrectAnswer { get; set; } = null!;

    public int? MaxScore { get; set; }

    public virtual Lesson Lesson { get; set; } = null!;

    public virtual ICollection<QuizAttempt> QuizAttempts { get; set; } = new List<QuizAttempt>();
}
