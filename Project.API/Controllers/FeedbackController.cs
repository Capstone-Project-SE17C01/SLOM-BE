using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.Core.Entities.General;
using Project.Core.Interfaces.IRepositories;

namespace Project.API.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FeedbackController : ControllerBase {
        private readonly IFeedbackRepository _repository;

        public FeedbackController(IFeedbackRepository repository) {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Feedback>>> GetAll() {
            var result = await _repository.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Feedback>> Get(int id) {
            var feedback = await _repository.GetByIdAsync(id);
            if (feedback == null) return NotFound();
            return Ok(feedback);
        }

        [HttpPost]
        public async Task<ActionResult<Feedback>> Create(Feedback feedback) {
            var created = await _repository.AddAsync(feedback);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Feedback feedback) {
            if (id != feedback.Id) return BadRequest();
            await _repository.UpdateAsync(feedback);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id) {
            await _repository.DeleteAsync(id);
            return NoContent();
        }
    }
}
