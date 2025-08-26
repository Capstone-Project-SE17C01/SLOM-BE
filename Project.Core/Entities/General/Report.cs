namespace Project.Core.Entities.General {
    public class Report {
        public Guid Id { get; set; }

        public string Title { get; set; } = null!;

        public string? Content { get; set; }

        public Guid ReportTypeId { get; set; }

        public ReportType ReportType { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool Status { get; set; }

        public Guid UserId { get; set; }

        public Guid? TransactionId { get; set; }

        public Payment? Transaction { get; set; }

        public Profile? User { get; set; }
    }
}
