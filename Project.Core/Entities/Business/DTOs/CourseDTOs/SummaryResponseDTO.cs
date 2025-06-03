using Project.Core.Entities.General;

namespace Project.Core.Entities.Business.DTOs.CourseDTOs {
    public class SummaryResponseDTO {
        public int TotalLessonsCompleted { get; set; }
        public int TotalLessons { get; set; }
        public int TotalModulesCompleted { get; set; }
        public int TotalModules { get; set; }
        public int TotalCoursesCompleted { get; set; }
        public int TotalCourse { get; set; }
        public Lesson? ActiveLesson { get; set; }
        public Activity? Activities { get; set; }
    }
}
