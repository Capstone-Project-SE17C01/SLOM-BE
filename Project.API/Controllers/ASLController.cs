using Microsoft.AspNetCore.Mvc;
using OpenCvSharp;
using Project.Core.Entities.Business.DTOs.ASLDTOs;
using Project.Infrastructure.Model.ASLPredictor;

namespace Project.API.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class ASLController : ControllerBase {
        private readonly ASLPredictor _predictor;
        private readonly ILogger<ASLController> _logger;

        public ASLController(ASLPredictor predictor, ILogger<ASLController> logger) {
            _predictor = predictor;
            _logger = logger;
        }

        [HttpPost("predict")]
        public ActionResult<PredictionResult> Predict(PredictionRequest request) {
            try {
                if (string.IsNullOrEmpty(request.ImageBase64))
                    return BadRequest("Image data is required");

                string base64Data = request.ImageBase64;
                if (base64Data.Contains(","))
                    base64Data = base64Data.Substring(base64Data.IndexOf(',') + 1);

                byte[] imageBytes = Convert.FromBase64String(base64Data);

                using Mat mat = Mat.FromImageData(imageBytes);

                _predictor.AddFrame(mat);

                var (word, confidence) = _predictor.Predict();

                return Ok(new PredictionResult {
                    Word = word.ToUpper(),
                    Confidence = confidence
                });
            } catch (Exception ex) {
                _logger.LogError(ex, "Error processing prediction request");
                return StatusCode(500, "Internal server error while processing image");
            }
        }

        [HttpPost("buffer")]
        public ActionResult BufferImage(BufferImageRequest request) {
            try {
                if (string.IsNullOrEmpty(request.ImageBase64))
                    return BadRequest("Image data is required");

                string base64Data = request.ImageBase64;
                if (base64Data.Contains(","))
                    base64Data = base64Data.Substring(base64Data.IndexOf(',') + 1);

                byte[] imageBytes = Convert.FromBase64String(base64Data);
                using Mat mat = Mat.FromImageData(imageBytes);

                _predictor.AddFrame(mat);

                return Ok();
            } catch (Exception ex) {
                _logger.LogError(ex, "Error processing buffer request");
                return StatusCode(500, "Internal server error while buffering image");
            }
        }

        [HttpGet("predict")]
        public ActionResult<PredictionResult> GetPrediction() {
            try {
                var (word, confidence) = _predictor.Predict();

                return Ok(new PredictionResult {
                    Word = word.ToUpper(),
                    Confidence = confidence
                });
            } catch (Exception ex) {
                _logger.LogError(ex, "Error processing get prediction request");
                return StatusCode(500, "Internal server error while getting prediction");
            }
        }
    }
}
