using Microsoft.AspNetCore.Mvc;
using Project.Core.Entities.Business.DTOs;
using Project.Core.Entities.Business.DTOs.ReminderDTOs;
using Project.Core.Entities.General;
using Project.Core.Interfaces.IMapper;
using Project.Core.Interfaces.IRepositories;

namespace Project.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReminderController : ControllerBase {
    private readonly IReminderRepository _remindRepo;
    private readonly IBaseMapper<CreateReminderDTO, Reminder> _mapper;

    public ReminderController(IReminderRepository remindRepo, IBaseMapper<CreateReminderDTO, Reminder> mapper) {
        _remindRepo = remindRepo;
        _mapper = mapper;
    }

    [HttpGet("GetReminder")]
    public async Task<IActionResult> GetReminder([FromQuery] string email) {
        if (string.IsNullOrWhiteSpace(email))
            return BadRequest(new APIResponse { errorMessages = ["Email is required"] });

        var reminder = await _remindRepo.GetReminderByEmailAsync(email, true);

        if (reminder == null)
            return NotFound(new APIResponse { errorMessages = ["No active reminder found for this email"] });

        return Ok(new APIResponse { result = reminder });
    }


    [HttpPost("SetupReminder")]
    public async Task<IActionResult> SetupReminder([FromBody] CreateReminderDTO requestDTO) {
        if (!ModelState.IsValid)
            return BadRequest(new APIResponse { errorMessages = ["Invalid request data"] });

        try {
            var existingReminder = await _remindRepo.GetReminderByEmailAsync(requestDTO.Email, false);

            if (existingReminder != null) {

                existingReminder.TimeToSend = requestDTO.TimeToSend;
                existingReminder.Message = requestDTO.Message;
                existingReminder.IsActive = requestDTO.IsActive;
                existingReminder.LastSentDate = null;

                await _remindRepo.Update(existingReminder);

                return Ok(new APIResponse { result = existingReminder });
            }

            var reminder = _mapper.MapModel(requestDTO);

            if (reminder == null)
                return BadRequest(new APIResponse { errorMessages = ["Mapping failed"] });

            reminder.Id = Guid.NewGuid();

            var created = await _remindRepo.Create(reminder);

            if (created == null)
                return StatusCode(500, new APIResponse { errorMessages = ["Failed to create reminder"] });

            return Ok(new APIResponse { result = created });
        }
        catch (Exception) {
            return StatusCode(500, new APIResponse { errorMessages = ["Unexpected server error"] });
        }
    }



}
