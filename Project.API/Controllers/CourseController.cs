using Microsoft.AspNetCore.Mvc;
using Project.Core.Entities.Business.DTOs;
using Project.Core.Entities.Business.DTOs.CourseDTOs;
using Project.Core.Interfaces.IRepositories;

namespace Project.API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase {
        private readonly IProfileRepository _profileRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IModuleRepository _moduleRepository;
        private readonly ILessonRepository _lessonRepository;
        private readonly IUserCourseProgressRepository _userCourseProgressRepository;
        private readonly IUserModuleProgressRepository _userModuleProgressRepository;
        private readonly IUserLessonProgressRepository _userLessonProgressRepository;

        public CourseController(
            IProfileRepository profileRepository,
            ICourseRepository courseRepository,
            IModuleRepository moduleRepository,
            ILessonRepository lessonRepository,
            IUserCourseProgressRepository userCourseProgressRepository,
            IUserModuleProgressRepository userModuleProgressRepository,
            IUserLessonProgressRepository userLessonProgressRepository) {
            _profileRepository = profileRepository;
            _courseRepository = courseRepository;
            _moduleRepository = moduleRepository;
            _lessonRepository = lessonRepository;
            _userCourseProgressRepository = userCourseProgressRepository;
            _userModuleProgressRepository = userModuleProgressRepository;
            _userLessonProgressRepository = userLessonProgressRepository;
        }

        [HttpGet("Summary")]
        public async Task<IActionResult> GetSummary(Guid userId, Guid courseId) {


            try {
                var totalCourses = await _courseRepository.CountAsync();
                var totalModules = await _moduleRepository.CountAsync();
                var totalLessons = await _lessonRepository.CountAsync();
                var totalCoursesCompleted = await _userCourseProgressRepository.CountCompletedAsync(courseId, userId);
                var totalModulesCompleted = await _userModuleProgressRepository.CountCompletedAsync(courseId, userId);
                var totalLessonsCompleted = await _userLessonProgressRepository.CountCompletedAsync(courseId, userId);
                var activeLessonEntry = await _userLessonProgressRepository.GetActiveLessonByUserIdAsync(userId);

                var recentLessonsCompleted = await _userLessonProgressRepository.CountLast7DaysCompletedLessonsAsync(userId);
                var recentModulesCompleted = await _userModuleProgressRepository.CountLast7DaysCompletedModulesAsync(userId);
                var recentCoursesCompleted = await _userCourseProgressRepository.CountLast7DaysCompletedCoursesAsync(userId);

                var summary = new SummaryResponseDTO {
                    totalCourse = totalCourses,
                    totalModules = totalModules,
                    totalLessons = totalLessons,
                    totalCoursesCompleted = totalCoursesCompleted,
                    totalModulesCompleted = totalModulesCompleted,
                    totalLessonsCompleted = totalLessonsCompleted,
                    activeLesson = activeLessonEntry,
                    activities = new Activity {
                        recentLessonsCompleted = recentLessonsCompleted,
                        recentModulesCompleted = recentModulesCompleted,
                        recentCoursesCompleted = recentCoursesCompleted
                    }
                };

                return Ok(new APIResponse { result = summary });
            } catch (Exception) {
                return BadRequest(new APIResponse { result = null, errorMessages = new List<string> { "Invalid request data for Get Summary" } });
            }
        }


        [HttpGet("GetListCourses")]
        public async Task<IActionResult> GetAllCourses(Guid userId) {
            try {
                var courses = await _courseRepository.GetAll();
                var learningCourses = await _userCourseProgressRepository.GetCoursesByUserIdAsync(userId);
                var remainingCourses = courses.Where(c => !learningCourses.Any(lc => lc.Id == c.Id)).ToList();
                var listCourseResponse = new ListCourseResponseDTO {
                    LearningCourses = learningCourses,
                    RemainingCourses = remainingCourses
                };
                return Ok(new APIResponse { result = listCourseResponse });
            } catch (Exception) {
                return BadRequest(new APIResponse { result = null, errorMessages = new List<string> { "Invalid request data for Get All Courses" } });
            }
        }
    }
}
