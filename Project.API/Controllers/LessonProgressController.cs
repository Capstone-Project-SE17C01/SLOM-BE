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
        public LessonProgressController(ApplicationDbContext context) {
            _userLessonProgressRepository = new UserLessonProgressRepository(context);
        }

        [HttpPost("AddNewProgress")]
        public async Task<IActionResult?> CreateNewUserLesson(Guid userId, Guid lessonId) {
            try {
                if (await _userLessonProgressRepository.CreateNewLessonProgress(userId, lessonId)) {
                    return Ok("Create progress success");
                } else {
                    return BadRequest(new APIResponse() { errorMessages = new List<string> { "Server Error" } });
                }
            } catch(Exception ex) {
                return BadRequest(new APIResponse() { errorMessages = new List<string> { ex.Message } });
            }
        }

        [HttpPut("CompleteCourse")]
        public async Task<IActionResult?> MarkCompleteCourse(Guid userId, Guid lessonId) {
            try {
                if (await _userLessonProgressRepository.CompleteLessons(userId, lessonId)) {
                    return Ok("Mark complete success");
                } else {
                    return BadRequest(new APIResponse() { errorMessages = new List<string> { "Server Error" } });
                }
            } catch (Exception ex) {
                return BadRequest(new APIResponse() { errorMessages = new List<string> { ex.Message } });
            }
        }
    }
}
