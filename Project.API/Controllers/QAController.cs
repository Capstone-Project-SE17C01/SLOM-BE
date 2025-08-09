using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.Core.Entities.Business.DTOs;
using Project.Core.Entities.Business.DTOs.AnswerDTOs;
using Project.Core.Entities.Business.DTOs.QuestionDTOs;
using Project.Core.Interfaces.IRepositories;

namespace Project.API.Controllers {
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class QAController : ControllerBase {
        private readonly IQuestionRepository _questionRepository;
        private readonly IAnswerRepository _answerRepository;

        public QAController(IQuestionRepository questionRepository, IAnswerRepository answerRepository) {
            _questionRepository = questionRepository;
            _answerRepository = answerRepository;
        }

        [HttpGet("GetQuestion")]
        public async Task<IActionResult> GetAllQuestion(int pageNumber, Guid userId, bool isCurrentUser, bool isAdmin = false) {
            try {
                var question = await _questionRepository.GetQuestionReponsePaginated(pageNumber, userId, isCurrentUser, isAdmin);
                if (question == null) {
                    return NotFound(new APIResponse {
                        errorMessages = new List<string> { "NoVideoSuggest" }
                    });
                }

                return Ok(new APIResponse {
                    result = question
                });
            }
            catch (Exception) {
                return StatusCode(500, new APIResponse {
                    errorMessages = new List<string> { "Error Video Suggest" }
                });
            }
        }

        [HttpGet("GetAnswer")]
        public async Task<IActionResult> GetAllAnswer(Guid questionId, int page) {
            try {
                var question = await _answerRepository.GetListAnswerByQuestion(questionId, page);
                if (question == null) {
                    return NotFound(new APIResponse {
                        errorMessages = new List<string> { "NoVideoSuggest" }
                    });
                }

                return Ok(new APIResponse {
                    result = question
                });
            }
            catch (Exception) {
                return StatusCode(500, new APIResponse {
                    errorMessages = new List<string> { "Error Video Suggest" }
                });
            }
        }

        [HttpPost("PostQuestion")]
        public async Task<IActionResult> PostNewQuestion(PostQuestionRequest question) {
            try {
                var createdQuestion = await _questionRepository.CreateQuestion(question);

                return Ok(new APIResponse {
                    result = createdQuestion
                });
            }
            catch (Exception) {
                return StatusCode(500, new APIResponse {
                    errorMessages = new List<string> { "Error Video Suggest" }
                });
            }
        }

        [HttpPost("PostAnswer")]
        public async Task<IActionResult> PostNewAnswer(PostAnswerRequest answer) {
            try {
                var createdAnswer = await _answerRepository.CreateAnswer(answer);

                return Ok(new APIResponse {
                    result = createdAnswer
                });
            }
            catch (Exception) {
                return StatusCode(500, new APIResponse {
                    errorMessages = new List<string> { "Error Video Suggest" }
                });
            }
        }

        [HttpPut("UpdateQuestion")]
        public async Task<IActionResult> UpdateQuestion(UpdateQuestionRequest question) {
            try {
                var createdAnswer = await _questionRepository.UpdateQuestion(question);

                return Ok(new APIResponse {
                    result = createdAnswer
                });
            }
            catch (Exception) {
                return StatusCode(500, new APIResponse {
                    errorMessages = new List<string> { "Error Video Suggest" }
                });
            }
        }

        [HttpDelete("DeleteQuestion/{questionId}")]
        public async Task<IActionResult> DeleteQuestion(Guid questionId) {
            try {
                var isDeleted = await _questionRepository.DeleteQuestion(questionId);

                if (!isDeleted) {
                    return NotFound(new APIResponse {
                        errorMessages = new List<string> { "Question not found" }
                    });
                }

                return Ok(new APIResponse {
                    result = "Question deleted successfully"
                });
            }
            catch (Exception) {
                return StatusCode(500, new APIResponse {
                    errorMessages = new List<string> { "Error deleting question" }
                });
            }
        }

        [HttpPut("UpdateAnswer")]
        public async Task<IActionResult> UpdateAnswer(UpdateAnswerRequest answer) {
            try {
                var updatedAnswer = await _answerRepository.UpdateAnswer(answer);

                return Ok(new APIResponse {
                    result = updatedAnswer
                });
            }
            catch (Exception ex) {
                return StatusCode(500, new APIResponse {
                    errorMessages = new List<string> { ex.Message }
                });
            }
        }

        [HttpDelete("DeleteAnswer/{answerId}")]
        public async Task<IActionResult> DeleteAnswer(Guid answerId) {
            try {
                var isDeleted = await _answerRepository.DeleteAnswer(answerId);

                if (!isDeleted) {
                    return NotFound(new APIResponse {
                        errorMessages = new List<string> { "Answer not found" }
                    });
                }

                return Ok(new APIResponse {
                    result = "Answer deleted successfully"
                });
            }
            catch (Exception) {
                return StatusCode(500, new APIResponse {
                    errorMessages = new List<string> { "Error deleting answer" }
                });
            }
        }

        [HttpGet("GetTags")]
        public async Task<IActionResult> GetTags() {
            try {
                var tags = await _questionRepository.GetTags();
                if (tags == null || !tags.Any()) {
                    return NotFound(new APIResponse {
                        errorMessages = new List<string> { "No tags found" }
                    });
                }
                return Ok(new APIResponse {
                    result = tags
                });
            }
            catch (Exception) {
                return StatusCode(500, new APIResponse {
                    errorMessages = new List<string> { "Error fetching tags" }
                });
            }
        }

        [HttpPost("GetQuestionByTag")]
        public async Task<IActionResult> GetQuestionByTag(string[] tags, int pageNumber, Guid userId, bool isCurrentUser, bool isAdmin = false) {
            try {
                var question = await _questionRepository.GetQuestionsByTag(tags, pageNumber, userId, isCurrentUser, isAdmin);
                if (question == null) {
                    return NotFound(new APIResponse {
                        errorMessages = new List<string> { "Not found question" }
                    });
                }
                return Ok(new APIResponse {
                    result = question
                });
            }
            catch (Exception) {
                return StatusCode(500, new APIResponse {
                    errorMessages = new List<string> { "Error fetching question" }
                });
            }
        }
    }
}
