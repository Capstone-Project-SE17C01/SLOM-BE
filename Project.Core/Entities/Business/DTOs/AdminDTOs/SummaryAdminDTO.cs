namespace Project.Core.Entities.Business.DTOs.AdminDTOs {
    public class SummaryAdminDTO {
        public int TotalUsers { get; set; }
        public int TotalCourses { get; set; }
        public int TotalMeetings { get; set; }
        public decimal TotalRevenue { get; set; }

        public int UserInUseToday { get; set; }
        public decimal RevenueToday { get; set; }
        public int NewUserToday { get; set; }

        public List<TimeSeriesItem<int>>? NewUserStats { get; set; }

        public List<TimeSeriesItem<decimal>>? RevenueStats { get; set; }
    }

    public class TimeSeriesItem<T> {
        public DateTime Date { get; set; }
        public required T Value { get; set; }
    }

}
