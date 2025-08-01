using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.Core.Entities.Business.DTOs;
using Project.Core.Entities.Business.DTOs.CourseDTOs;
using Project.Core.Entities.General;
using Project.Core.Interfaces.IRepositories;

namespace Project.API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
                    TotalCourse = totalCourses,
                    TotalModules = totalModules,
                    TotalLessons = totalLessons,
                    TotalCoursesCompleted = totalCoursesCompleted,
                    TotalModulesCompleted = totalModulesCompleted,
                    TotalLessonsCompleted = totalLessonsCompleted,
                    ActiveLesson = activeLessonEntry,
                    Activities = new Activity {
                        RecentLessonsCompleted = recentLessonsCompleted,
                        RecentModulesCompleted = recentModulesCompleted,
                        RecentCoursesCompleted = recentCoursesCompleted
                    }
                };

                return Ok(new APIResponse { result = summary });
            }
            catch (Exception) {
                return BadRequest(new APIResponse { result = null, errorMessages = new List<string> { "Invalid request data for Get Summary" } });
            }
        }


        [HttpGet("GetListCourses")]
        public async Task<IActionResult> GetAllCourses(Guid userId) {
            try {
                var courses = await _courseRepository.GetAll();
                var learningCourses = await _userCourseProgressRepository.GetCoursesByUserIdAsync(userId);
                var remainingCourses = courses.Where(c => !learningCourses.Any(lc => lc.Id == c.Id) && c.IsPublished).ToList();
                var listCourseResponse = new ListCourseResponseDTO {
                    LearningCourses = learningCourses,
                    RemainingCourses = remainingCourses
                };
                return Ok(new APIResponse { result = listCourseResponse });
            }
            catch (Exception) {
                return BadRequest(new APIResponse { result = null, errorMessages = new List<string> { "Invalid request data for Get All Courses" } });
            }
        }

        [HttpGet("GetAllCourse")]
        public async Task<IActionResult> GetAllCourse() {
            try {
                var courses = await _courseRepository.GetAll();
                return Ok(new APIResponse { result = courses });
            }
            catch (Exception ex) {
                return BadRequest(new APIResponse { result = null, errorMessages = new List<string> { ex.Message } });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourseById(Guid id) {
            try {
                var course = await _courseRepository.GetById(id);
                if (course == null) {
                    return NotFound(new APIResponse { result = null, errorMessages = new List<string> { "Course not found" } });
                }
                return Ok(new APIResponse { result = course });
            }
            catch (Exception ex) {
                return BadRequest(new APIResponse { result = null, errorMessages = new List<string> { ex.Message } });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCourse([FromBody] CourseRequestDTO courseDto) {
            try {
                if (courseDto == null) {
                    return BadRequest(new APIResponse { result = null, errorMessages = new List<string> { "Invalid course data" } });
                }
                var course = new Course {
                    Id = Guid.NewGuid(),
                    Title = courseDto.Title,
                    Description = courseDto.Description,
                    IsPublished = courseDto.IsPublished,
                    ThumbnailUrl = courseDto.ThumbnailUrl,
                    CreatedAt = DateTime.UtcNow
                };
                await _courseRepository.Create(course);
                return Ok(new APIResponse { result = course });
            }
            catch (Exception ex) {
                return BadRequest(new APIResponse { result = null, errorMessages = new List<string> { ex.Message } });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCourse([FromBody] CourseRequestDTO courseDto) {
            try {
                if (courseDto == null) {
                    return BadRequest(new APIResponse { result = null, errorMessages = new List<string> { "Invalid course data" } });
                }
                var course = await _courseRepository.GetById(courseDto.Id);
                if (course == null) {
                    return NotFound(new APIResponse { result = null, errorMessages = new List<string> { "Course not found" } });
                }
                course.Title = courseDto.Title;
                course.Description = courseDto.Description;
                course.IsPublished = courseDto.IsPublished;
                course.ThumbnailUrl = courseDto.ThumbnailUrl;
                course.UpdatedAt = DateTime.UtcNow;
                await _courseRepository.Update(course);
                return Ok(new APIResponse { result = course });
            }
            catch (Exception ex) {
                return BadRequest(new APIResponse { result = null, errorMessages = new List<string> { ex.Message } });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(Guid id) {
            try {
                var course = await _courseRepository.GetById(id);
                if (course == null) {
                    return NotFound(new APIResponse { result = null, errorMessages = new List<string> { "Course not found" } });
                }
                await _courseRepository.Delete(course);
                return Ok(new APIResponse { result = "Course deleted successfully" });
            }
            catch (Exception ex) {
                return BadRequest(new APIResponse { result = null, errorMessages = new List<string> { ex.Message } });
            }
        }
    }
}
