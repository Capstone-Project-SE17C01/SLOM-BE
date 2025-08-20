using Project.Core.Entities.General;

namespace Project.Core.Entities.Business.DTOs.CourseDTOs {
    public class SummaryResponseDTO {
        public int TotalQuizzesCompleted { get; set; }
        public int TotalLessonsLearned { get; set; }
        public int TotalLessons { get; set; }
        public int TotalQuizzes { get; set; }
        public int TotalModulesCompleted { get; set; }
        public int TotalModules { get; set; }
        public Lesson? ActiveLesson { get; set; }
        public Activity? Activities { get; set; }
    }
}
