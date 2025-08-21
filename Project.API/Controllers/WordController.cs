using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.Core.Entities.Business.DTOs;
using Project.Core.Entities.Business.DTOs.WordDTOs;
using Project.Core.Entities.General;
using Project.Core.Interfaces.IRepositories;

namespace Project.API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WordController : ControllerBase {
        private readonly IWordRepository _wordRepository;

        public WordController(IWordRepository wordRepository) {
            _wordRepository = wordRepository;
        }

        [HttpGet("GetListWordLesson")]
        public async Task<APIResponse> GetListWordLesson(Guid lessonId) {
            List<Word> words = await _wordRepository.GetWordByLessonId(lessonId);
            return new APIResponse() { errorMessages = null, result = words };
        }

        [HttpGet]
        public async Task<APIResponse> GetAllWords() {
            try {
                var words = await _wordRepository.GetAllWords();
                return new APIResponse { errorMessages = null, result = words };
            }
            catch (Exception ex) {
                return new APIResponse { errorMessages = new List<string> { ex.Message }, result = null };
            }
        }

        [HttpGet("{id}")]
        public async Task<APIResponse> GetWordById(Guid id) {
            Word? word = await _wordRepository.GetById(id);
            if (word == null) {
                return new APIResponse() { errorMessages = new List<string> { "Word not found" }, result = null };
            }
            return new APIResponse() { errorMessages = null, result = word };
        }

        [HttpPost]
        public async Task<APIResponse> CreateWord([FromBody] WordRequestDTO wordRequestDTO) {
            if (wordRequestDTO == null) {
                return new APIResponse() { errorMessages = new List<string> { "Invalid word data" }, result = null };
            }
            Word newWord = new Word {
                Id = Guid.NewGuid(),
                LessonId = wordRequestDTO.LessonId,
                Text = wordRequestDTO.Text,
                VideoSrc = wordRequestDTO.VideoSrc
            };
            await _wordRepository.Create(newWord);
            return new APIResponse() { errorMessages = null, result = newWord };
        }

        [HttpPut]
        public async Task<APIResponse> UpdateWord([FromBody] WordRequestDTO wordRequestDTO) {
            if (wordRequestDTO == null) {
                return new APIResponse() { errorMessages = new List<string> { "Invalid word data" }, result = null };
            }
            Word? existingWord = await _wordRepository.GetById(wordRequestDTO.Id);
            if (existingWord == null) {
                return new APIResponse() { errorMessages = new List<string> { "Word not found" }, result = null };
            }
            existingWord.Text = wordRequestDTO.Text;
            existingWord.VideoSrc = wordRequestDTO.VideoSrc;
            existingWord.LessonId = wordRequestDTO.LessonId;
            await _wordRepository.Update(existingWord);
            return new APIResponse() { errorMessages = null, result = existingWord };
        }

        [HttpDelete("{id}")]
        public async Task<APIResponse> DeleteWord(Guid id) {
            Word? existingWord = await _wordRepository.GetById(id);
            if (existingWord == null) {
                return new APIResponse() { errorMessages = new List<string> { "Word not found" }, result = null };
            }
            await _wordRepository.Delete(existingWord);
            return new APIResponse() { errorMessages = null, result = "Word deleted successfully" };
        }
    }
}
