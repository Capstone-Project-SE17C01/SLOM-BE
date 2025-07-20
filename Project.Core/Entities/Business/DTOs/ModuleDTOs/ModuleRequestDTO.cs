namespace Project.Core.Entities.Business.DTOs.ModuleDTOs {
    public class ModuleRequestDTO {
        public Guid Id { get; set; }

        public Guid CourseId { get; set; }

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public int OrderNumber { get; set; }
    }
}
