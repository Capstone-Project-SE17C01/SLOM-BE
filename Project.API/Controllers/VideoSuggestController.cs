using Microsoft.AspNetCore.Mvc;
using Project.Core.Entities.Business.DTOs;
using Project.Core.Entities.Business.DTOs.VideoSuggestDTOs;
using Project.Core.Interfaces.IRepositories;

namespace Project.API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class VideoSuggestController : ControllerBase {
        private readonly IVideoSuggestRepository _videoRepo;
        private readonly IUserLessonProgressRepository _userLessonProgressRepo;

        public VideoSuggestController(IVideoSuggestRepository videoRepo, IUserLessonProgressRepository userLessonProgressRepo) {
            _videoRepo = videoRepo;
            _userLessonProgressRepo = userLessonProgressRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetVideoSuggest([FromQuery] VideoSuggestRequestDTO requestDTO) {
            try {
                var videoSuggests = await _videoRepo.GetVideoSuggestsByUserId(requestDTO);
                if (videoSuggests == null || !videoSuggests.Any()) {
                    return NotFound(new APIResponse {
                        errorMessages = new List<string> { "NoVideoSuggest" }
                    });
                }

                return Ok(new APIResponse {
                    result = videoSuggests
                });
            } catch (Exception) {
                return StatusCode(500, new APIResponse {
                    errorMessages = new List<string> { "Error Video Suggest" }
                });
            }

        }
    }
}
