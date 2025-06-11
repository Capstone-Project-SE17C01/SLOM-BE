namespace Project.Core.Entities.Business.DTOs.ReportDTOs {
    public class CreateReportRequestDTO {
        public string Title { get; set; } = null!;

        public string? Content { get; set; }

        public Guid ReportTypeId { get; set; }

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        public bool Status { get; set; }

        public Guid UserId { get; set; }
    }
}
