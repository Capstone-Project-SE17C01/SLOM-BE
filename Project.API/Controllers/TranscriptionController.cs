using Microsoft.AspNetCore.Mvc;
using Project.Core.Entities.Business.DTOs;
using AssemblyAI;
using AssemblyAI.Transcripts;

namespace Project.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TranscriptionController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TranscriptionController> _logger;

        public TranscriptionController(IConfiguration configuration, ILogger<TranscriptionController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> TranscribeVideo([FromBody] string videoUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(videoUrl))
                {
                    return BadRequest(new APIResponse { 
                        errorMessages = new List<string> { "Video URL is required" } 
                    });
                }

                var apiKey = _configuration["AssemblyAI:ApiKey"];
                if (string.IsNullOrEmpty(apiKey))
                {
                    return StatusCode(500, new APIResponse { 
                        errorMessages = new List<string> { "AssemblyAI API key not configured" } 
                    });
                }

                var client = new AssemblyAIClient(apiKey);

                var transcriptParams = new TranscriptParams
                {
                    AudioUrl = videoUrl,
                    LanguageDetection = true
                };

                var transcript = await client.Transcripts.TranscribeAsync(transcriptParams);

                return Ok(new APIResponse {
                    result = new
                    {
                        Text = transcript.Text,
                        AudioDuration = transcript.AudioDuration,
                        Confidence = transcript.Confidence
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during video transcription");
                return StatusCode(500, new APIResponse {
                    errorMessages = new List<string> { "Internal server error during transcription", ex.Message }
                });
            }
        }
    }
} 
