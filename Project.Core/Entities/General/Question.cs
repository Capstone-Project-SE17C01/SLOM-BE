namespace Project.Core.Entities.General {
    public class Question {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CreatorId { get; set; }
        public string Content { get; set; } = null!;
        public string Images { get; set; } = null!;
        public Profile Creator { get; set; } = new Profile();
        public int AnswersAmount { get; set; } = 0;
        public string Privacy { get; set; } = null!;

        public ICollection<Answer> Answers { get; set; } = new List<Answer>();
    }
}
