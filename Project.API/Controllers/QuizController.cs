using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.Core.Entities.Business.DTOs;
using Project.Core.Entities.Business.DTOs.QuizDTOs;
using Project.Core.Entities.General;
using Project.Core.Interfaces.IRepositories;

namespace Project.API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class QuizController : ControllerBase {
        private readonly IQuizRepository _quizRepository;
        private readonly IQuizOptionRepository _quizOptionRepository;

        public QuizController(IQuizRepository quizRepository, IQuizOptionRepository quizOptionRepository) {
            _quizRepository = quizRepository;
            _quizOptionRepository = quizOptionRepository;
        }

        [HttpGet("GetListQuizLesson")]
        public async Task<APIResponse> GetListQuizLesson(Guid lessonId) {
            List<Quiz> quizzes = await _quizRepository.GetAllQuizByLessonId(lessonId);
            return new APIResponse() { errorMessages = null, result = quizzes };
        }

        [HttpGet]
        public async Task<APIResponse> GetAllQuizzes() {
            try {
                var quizzes = await _quizRepository.GetAllQuiz();
                return new APIResponse { errorMessages = null, result = quizzes };
            }
            catch (Exception ex) {
                return new APIResponse { errorMessages = new List<string> { ex.Message }, result = null };
            }
        }

        [HttpGet("{id}")]
        public async Task<APIResponse> GetQuizById(Guid id) {
            Quiz? quiz = await _quizRepository.GetById(id);
            if (quiz == null) {
                return new APIResponse() { errorMessages = new List<string> { "Quiz not found" }, result = null };
            }
            return new APIResponse() { errorMessages = null, result = quiz };
        }

        [HttpPost]
        public async Task<APIResponse> CreateQuiz([FromBody] QuizRequestDTO quizRequestDTO) {
            if (quizRequestDTO == null) {
                return new APIResponse() { errorMessages = new List<string> { "Invalid quiz data" }, result = null };
            }
            var quizId = Guid.NewGuid();

            List<QuizOption> quizOptions = quizRequestDTO.QuizOptions.Select(x => new QuizOption {
                Id = Guid.NewGuid(),
                IsCorrect = quizRequestDTO.CorrectAnswer == x,
                Text = x
            }).ToList();

            await _quizOptionRepository.CreateRange(quizOptions);

            Quiz newQuiz = new Quiz {
                Id = quizId,
                LessonId = quizRequestDTO.LessonId,
                Question = quizRequestDTO.Question,
                CorrectAnswer = quizRequestDTO.CorrectAnswer,
                Explanation = quizRequestDTO.Explanation,
                MaxScore = quizRequestDTO.MaxScore,
                CreatedAt = DateTime.UtcNow,
                QuizOptions = quizOptions
            };
            await _quizRepository.Create(newQuiz);
            return new APIResponse() { errorMessages = null, result = newQuiz };
        }

        [HttpPut]
        public async Task<APIResponse> UpdateQuiz([FromBody] QuizRequestDTO quizRequestDTO) {
            if (quizRequestDTO == null) {
                return new APIResponse() { errorMessages = new List<string> { "Invalid quiz data" }, result = null };
            }
            Quiz? existingQuiz = await _quizRepository.GetById(quizRequestDTO.Id);
            if (existingQuiz == null) {
                return new APIResponse() { errorMessages = new List<string> { "Quiz not found" }, result = null };
            }

            var existingOptions = await _quizRepository.GetById(quizRequestDTO.Id);
            var options = existingOptions.QuizOptions;

            if (options.Any()) {
                foreach (var option in options) {
                    await _quizOptionRepository.Delete(option);
                }
            }

            List<QuizOption> quizOptions = quizRequestDTO.QuizOptions.Select(x => new QuizOption {
                Id = Guid.NewGuid(),
                QuizId = quizRequestDTO.Id,
                IsCorrect = quizRequestDTO.CorrectAnswer == x,
                Text = x,
            }).ToList();

            await _quizOptionRepository.CreateRange(quizOptions);

            existingQuiz.Question = quizRequestDTO.Question;
            existingQuiz.CorrectAnswer = quizRequestDTO.CorrectAnswer;
            existingQuiz.Explanation = quizRequestDTO.Explanation;
            existingQuiz.MaxScore = quizRequestDTO.MaxScore;
            existingQuiz.QuizOptions = quizOptions;
            await _quizRepository.Update(existingQuiz);
            return new APIResponse() { errorMessages = null, result = existingQuiz };
        }

        [HttpDelete("{id}")]
        public async Task<APIResponse> DeleteQuiz(Guid id) {
            Quiz? existingQuiz = await _quizRepository.GetById(id);
            if (existingQuiz == null) {
                return new APIResponse() { errorMessages = new List<string> { "Quiz not found" }, result = null };
            }
            await _quizRepository.Delete(existingQuiz);
            return new APIResponse() { errorMessages = null, result = "Quiz deleted successfully" };
        }
    }
}
