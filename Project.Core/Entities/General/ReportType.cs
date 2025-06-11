namespace Project.Core.Entities.General {
    public class ReportType {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public ICollection<Report> Reports { get; set; } = new List<Report>();
    }

}
