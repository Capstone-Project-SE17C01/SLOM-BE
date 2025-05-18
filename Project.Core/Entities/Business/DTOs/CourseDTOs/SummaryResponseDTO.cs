using Project.Core.Entities.General;

namespace Project.Core.Entities.Business.DTOs.CourseDTOs {
    public class SummaryResponseDTO {
        public int totalLessonsCompleted { get; set; }
        public int totalLessons { get; set; }
        public int totalModulesCompleted { get; set; }
        public int totalModules { get; set; }
        public int totalCoursesCompleted { get; set; }
        public int totalCourse { get; set; }
        public Lesson? activeLesson { get; set; }
        public Activity? activities { get; set; }
    }
}
