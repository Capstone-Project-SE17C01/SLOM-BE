namespace Project.Core.Entities.Business.DTOs.CourseDTOs {
    public class Activity {
        public int RecentLessonsCompleted { get; set; }
        public int RecentModulesCompleted { get; set; }
        public int RecentCoursesCompleted { get; set; }

        public Activity() {
            RecentLessonsCompleted = 0;
            RecentModulesCompleted = 0;
            RecentCoursesCompleted = 0;
        }
    }
}
