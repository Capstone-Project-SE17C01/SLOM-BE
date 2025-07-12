using Microsoft.AspNetCore.Mvc;
using Project.Core.Entities.Business.DTOs;
using Project.Core.Entities.General;
using Project.Core.Interfaces.IRepositories;

namespace Project.API.Controllers {
    [Route("api/Lesson")]
    [ApiController]
    public class LessonController : ControllerBase {
        private readonly IUserLessonProgressRepository _userLessonProgressRepository;
        private readonly IWordRepository _wordRepository;
        private readonly IQuizRepository _quizRepository;
        
        public LessonController(
            IUserLessonProgressRepository userLessonProgressRepository,
            IWordRepository wordRepository,
            IQuizRepository quizRepository) {
            _userLessonProgressRepository = userLessonProgressRepository;
            _wordRepository = wordRepository;
            _quizRepository = quizRepository;
        }

        [HttpGet("OngoingLesson")]
        public async Task<APIResponse> GetOngoingUserLesson(Guid userId) {
            Lesson? lesson = await _userLessonProgressRepository.GetActiveLessonByUserIdAsync(userId);
            return new APIResponse() { errorMessages = null, result = lesson };
        }

        [HttpGet("GetListLearnedLesson")]
        public async Task<APIResponse> GetLearnedLesson(Guid userId) {
            List<Lesson> lessons = await _userLessonProgressRepository.GetLearnedLessons(userId);
            return new APIResponse() { errorMessages = null, result = lessons };
        }

        [HttpGet("GetListWordLesson")]
        public async Task<APIResponse> GetListWordLesson(Guid lessonId) {
            List<Word> words = await _wordRepository.GetWordByLessonId(lessonId);
            return new APIResponse() { errorMessages = null, result = words };
        }

        [HttpGet("GetListQuizLesson")]
        public async Task<APIResponse> GetListQuizLesson(Guid lessonId) {
            List<Quiz> quizzes = await _quizRepository.GetAllQuizByLessonId(lessonId);
            return new APIResponse() { errorMessages = null, result = quizzes };
        }
    }
}
