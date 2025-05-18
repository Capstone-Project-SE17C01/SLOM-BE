using Project.Core.Entities.General;

namespace Project.Core.Entities.Business.DTOs.CourseDTOs {
    public class ListCourseResponseDTO {
        public required List<Course> LearningCourses { get; set; }
        public required List<Course> RemainingCourses { get; set; }
    }
}
