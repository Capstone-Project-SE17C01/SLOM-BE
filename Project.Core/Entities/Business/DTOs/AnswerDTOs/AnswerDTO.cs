namespace Project.Core.Entities.Business.DTOs.AnswerDTOs {
    public class Author {
        public string Username { get; set; } = null!;
        public string ProfileImage { get; set; } = null!;
    }

    public class AnswerResponse {
        public Guid AnswerId { get; set; }
        public Guid QuestionId { get; set; }
        public DateTime CreatedAt { get; set; }
        public Author Author { get; set; } = null!;
        public string Content { get; set; } = null!;
        public bool IsFull { get; set; }
        public List<string> Images { get; set; } = new List<string>();
    }
}
