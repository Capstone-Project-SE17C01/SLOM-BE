using Microsoft.AspNetCore.Mvc;
using Project.Core.Entities.Business.DTOs;
using Project.Core.Interfaces.IRepositories;
using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;

namespace Project.API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class LessonProgressController : ControllerBase {
        private readonly IUserLessonProgressRepository _userLessonProgressRepository;
        private readonly IUserModuleProgressRepository _userModuleProgressRepository;
        private readonly IUserCourseProgressRepository _userCourseProgressRepository;
        private readonly ILessonRepository _lessonRepository;

        public LessonProgressController(ApplicationDbContext context) {
            _userLessonProgressRepository = new UserLessonProgressRepository(context);
            _userModuleProgressRepository = new UserModuleProgressRepository(context);
            _userCourseProgressRepository = new UserCourseProgressRepository(context);
            _lessonRepository = new LessonRepository(context);
        }

        [HttpPost("AddNewProgress")]
        public async Task<IActionResult?> CreateNewUserLesson(Guid userId, Guid lessonId) {
            try {
                if (await _userLessonProgressRepository.CreateNewLessonProgress(userId, lessonId)) {
                    return Ok(new APIResponse() {
                        errorMessages = null,
                        result = "Create new lesson progress success"
                    });
                } else {
                    return BadRequest(new APIResponse() { errorMessages = new List<string> { "Server Error" } });
                }
            } catch (Exception ex) {
                return BadRequest(new APIResponse() { errorMessages = new List<string> { ex.Message } });
            }
        }

        [HttpPut("CompleteLesson")]
        public async Task<IActionResult?> MarkCompleteLesson(Guid userId, Guid lessonId) {
            try {
                if (await _userLessonProgressRepository.CompleteLessons(userId, lessonId)) {
                    return Ok(new APIResponse() {
                        errorMessages = null,
                        result = "Mark complete success"
                    });
                } else {
                    return BadRequest(new APIResponse() { errorMessages = new List<string> { "Server Error" } });
                }
            } catch (Exception ex) {
                return BadRequest(new APIResponse() { errorMessages = new List<string> { ex.Message } });
            }
        }

        [HttpPut("LearnedLesson")]
        public async Task<IActionResult?> MarkLearnLesson(Guid userId, Guid lessonId) {
            try {
                if (await _userLessonProgressRepository.LearnedLessons(userId, lessonId)) {
                    return Ok(new APIResponse() {
                        errorMessages = null,
                        result = "Mark learned success"
                    });
                } else {
                    return BadRequest(new APIResponse() { errorMessages = new List<string> { "Server Error" } });
                }
            } catch (Exception ex) {
                return BadRequest(new APIResponse() { errorMessages = new List<string> { ex.Message } });
            }
        }
    }
}
