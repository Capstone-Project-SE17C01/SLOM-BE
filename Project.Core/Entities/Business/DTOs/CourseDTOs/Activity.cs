namespace Project.Core.Entities.Business.DTOs.CourseDTOs {
    public class Activity {
        public int recentLessonsCompleted { get; set; }
        public int recentModulesCompleted { get; set; }
        public int recentCoursesCompleted { get; set; }

        public Activity() {
            recentLessonsCompleted = 0;
            recentModulesCompleted = 0;
            recentCoursesCompleted = 0;
        }
    }
}
