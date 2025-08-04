namespace Project.Core.Entities.Business.DTOs.QuestionDTOs {
    public class Author {
        public string Username { get; set; } = null!;
        public string ProfileImage { get; set; } = null!;
    }

    public class QuestionResponse {
        public Guid QuestionId { get; set; }
        public DateTime CreatedAt { get; set; }
        public Author Author { get; set; } = null!;
        public string Content { get; set; } = null!;
        public List<string> Images { get; set; } = new List<string>();
        public int AnswerAmount { get; set; }
        public bool IsFull { get; set; }
        public string Privacy { get; set; } = null!;
    }
}
