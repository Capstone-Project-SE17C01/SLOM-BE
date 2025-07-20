namespace Project.Core.Entities.Business.DTOs.CourseDTOs {
    public class CourseRequestDTO {
        public Guid Id { get; set; }

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public string? DifficultyLevel { get; set; }

        public string? ThumbnailUrl { get; set; }

        public bool IsPublished { get; set; } = true;
    }
}
