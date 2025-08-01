using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.Core.Entities.Business.DTOs;
using Project.Core.Entities.Business.DTOs.LessonDTOs;
using Project.Core.Entities.General;
using Project.Core.Interfaces.IRepositories;

namespace Project.API.Controllers {
    [Route("api/Lesson")]
    [ApiController]
    [Authorize]
    public class LessonController : ControllerBase {
        private readonly IUserLessonProgressRepository _userLessonProgressRepository;
        private readonly IWordRepository _wordRepository;
        private readonly ILessonRepository _lessonRepository;

        public LessonController(
            IUserLessonProgressRepository userLessonProgressRepository,
            IWordRepository wordRepository,
            ILessonRepository lessonRepository) {
            _userLessonProgressRepository = userLessonProgressRepository;
            _wordRepository = wordRepository;
            _lessonRepository = lessonRepository;
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

        [HttpGet]
        public async Task<IActionResult> GetAllLessons() {
            try {
                var lessons = await _lessonRepository.GetAllLessonHasModule();
                return Ok(new APIResponse { result = lessons });
            }
            catch (Exception ex) {
                return BadRequest(new APIResponse { result = null, errorMessages = new List<string> { ex.Message } });
            }
        }

        [HttpGet("{id}")]
        public async Task<APIResponse> GetLessonById(Guid id) {
            Lesson? lesson = await _lessonRepository.GetById(id);
            if (lesson == null) {
                return new APIResponse() { errorMessages = new List<string> { "Lesson not found" }, result = null };
            }
            return new APIResponse() { errorMessages = null, result = lesson };
        }

        [HttpPost]
        public async Task<APIResponse> CreateNewLesson(LessonRequestDTO lessonRequestDTO) {
            if (lessonRequestDTO == null) {
                return new APIResponse() { errorMessages = new List<string> { "Invalid lesson data" }, result = null };
            }
            Lesson lesson = new Lesson {
                Id = Guid.NewGuid(),
                ModuleId = lessonRequestDTO.ModuleId,
                Title = lessonRequestDTO.Title,
                Content = lessonRequestDTO.Content,
                VideoUrl = lessonRequestDTO.VideoUrl,
                DurationMinutes = lessonRequestDTO.DurationMinutes,
                OrderNumber = lessonRequestDTO.OrderNumber,
                CreatedAt = DateTime.UtcNow
            };
            await _lessonRepository.Create(lesson);
            return new APIResponse() { result = lesson };
        }

        [HttpPut]
        public async Task<APIResponse> UpdateLesson(LessonRequestDTO lessonRequestDTO) {
            if (lessonRequestDTO == null) {
                return new APIResponse() { errorMessages = new List<string> { "Invalid lesson data" }, result = null };
            }
            Lesson? existingLesson = await _lessonRepository.GetById(lessonRequestDTO.Id);
            if (existingLesson == null) {
                return new APIResponse() { errorMessages = new List<string> { "Lesson not found" }, result = null };
            }
            existingLesson.ModuleId = lessonRequestDTO.ModuleId;
            existingLesson.Title = lessonRequestDTO.Title;
            existingLesson.Content = lessonRequestDTO.Content;
            existingLesson.VideoUrl = lessonRequestDTO.VideoUrl;
            existingLesson.DurationMinutes = lessonRequestDTO.DurationMinutes;
            existingLesson.OrderNumber = lessonRequestDTO.OrderNumber;
            await _lessonRepository.Update(existingLesson);
            return new APIResponse() { result = existingLesson };
        }

        [HttpDelete("{id}")]
        public async Task<APIResponse> DeleteLesson(Guid id) {
            Lesson? lesson = await _lessonRepository.GetByIdForDelete(id);
            // Check if the lesson exists and has no associated quizzes or words
            if (lesson == null || lesson.Quizzes.Any() || lesson.Words.Any()) {
                return new APIResponse() { errorMessages = new List<string> { "Lesson has associated quizzes/words" }, result = null };
            }

            if (lesson == null) {
                return new APIResponse() { errorMessages = new List<string> { "Lesson not found" }, result = null };
            }
            await _lessonRepository.Delete(lesson);
            return new APIResponse() { result = "Lesson deleted successfully" };
        }
    }
}
