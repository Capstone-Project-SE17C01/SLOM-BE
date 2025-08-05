namespace Project.Core.Entities.General {
    public class Answer {
        public Guid Id { get; set; }
        public Guid QuestionId { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CreatorId { get; set; }
        public string Content { get; set; } = null!;
        public string Images { get; set; } = null!;
        public Profile Creator { get; set; } = new Profile();
        public Question Question { get; set; } = new Question();
    }
}
