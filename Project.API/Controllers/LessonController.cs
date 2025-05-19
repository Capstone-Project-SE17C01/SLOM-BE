using Microsoft.AspNetCore.Mvc;
using Project.Core.Entities.General;
using Project.Core.Interfaces.IRepositories;
using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;

namespace Project.API.Controllers {
    [Route("api/Lesson")]
    [ApiController]
    public class LessonController : ControllerBase {
        private readonly IUserLessonProgressRepository _userLessonProgressRepository;
        public LessonController(ApplicationDbContext context) {
            _userLessonProgressRepository = new UserLessonProgressRepository(context);
        }

        [HttpGet("OngoingLesson")]
        public async Task<UserLessonProgress?> GetOngoingUserLesson(Guid userId) {
            return await _userLessonProgressRepository.GetActiveLessonByUserIdAsync(userId);
        }

        [HttpGet("LearnedLesson")]
        public async Task<List<UserLessonProgress>> GetLearnedLesson(Guid userId) {
            return await _userLessonProgressRepository.GetLearnedLessons(userId);
        }
    }
}
