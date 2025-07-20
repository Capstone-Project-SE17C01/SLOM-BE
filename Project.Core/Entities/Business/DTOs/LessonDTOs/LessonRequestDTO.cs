namespace Project.Core.Entities.Business.DTOs.LessonDTOs {
    public class LessonRequestDTO {
        public Guid Id { get; set; }
        public Guid ModuleId { get; set; }

        public string Title { get; set; } = null!;

        public string? Content { get; set; }

        public string? VideoUrl { get; set; }

        public int? DurationMinutes { get; set; }

        public int OrderNumber { get; set; }
    }
}
