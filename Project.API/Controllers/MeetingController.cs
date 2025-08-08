using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.Core.Entities.Business.DTOs.MeetingDTOs;
using Project.Core.Entities.General;
using Project.Core.Interfaces.IRepositories;
using Project.Core.Interfaces.IServices;

namespace Project.API.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MeetingController : ControllerBase {
        private readonly IMeetingRepository _meetingRepository;
        private readonly IEmailService _emailService;

        public MeetingController(IMeetingRepository meetingRepository, IEmailService emailService) {
            _meetingRepository = meetingRepository;
            _emailService = emailService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateMeeting([FromBody] MeetingCreateDto meetingDto) {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = meetingDto.UserId;
            if (string.IsNullOrEmpty(userId))
                return BadRequest("User ID is required");
            var meeting = new Meeting {
                Id = Guid.NewGuid(),
                HostId = Guid.Parse(userId),
                Title = meetingDto.Title,
                Description = meetingDto.Description,
                StartTime = meetingDto.IsImmediate ? DateTime.UtcNow : DateTime.SpecifyKind(meetingDto.StartTime.GetValueOrDefault(), DateTimeKind.Utc),
                EndTime = meetingDto.Duration.HasValue ?
                    (meetingDto.IsImmediate ? DateTime.UtcNow.AddMinutes(meetingDto.Duration.Value) :
                     DateTime.SpecifyKind(meetingDto.StartTime?.AddMinutes(meetingDto.Duration.Value) ?? DateTime.UtcNow, DateTimeKind.Utc))
                    : null,
                Status = meetingDto.IsImmediate ? "Active" : "Scheduled",
                IsPrivate = meetingDto.IsPrivate,
                GuestCode = meetingDto.IsPrivate ? GenerateRandomCode() : null,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _meetingRepository.CreateMeetingAsync(meeting);
            return Ok(new {
                id = result.Id,
                title = result.Title,
                description = result.Description,
                startTime = result.StartTime,
                endTime = result.EndTime,
                status = result.Status,
                isPrivate = result.IsPrivate,
                guestCode = result.GuestCode
            });
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveMeetings([FromQuery] string userId) {
            var meetings = string.IsNullOrEmpty(userId)
                ? await _meetingRepository.GetActiveMeetingsAsync()
                : await _meetingRepository.GetActiveMeetingsAsync(Guid.Parse(userId));

            return Ok(meetings.Select(m => new {
                id = m.Id,
                title = m.Title,
                description = m.Description,
                hostId = m.HostId,
                hostName = m.Host?.Username,
                participantCount = m.Participants.Count,
                startTime = m.StartTime,
                endTime = m.EndTime,
                status = m.Status,
                isPrivate = m.IsPrivate,
                isDeleted = m.IsDeleted
            }));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMeeting(Guid id) {
            var meeting = await _meetingRepository.GetMeetingByIdAsync(id);
            if (meeting == null)
                return NotFound();

            return Ok(new {
                id = meeting.Id,
                title = meeting.Title,
                description = meeting.Description,
                hostId = meeting.HostId,
                hostName = meeting.Host?.Username,
                participants = meeting.Participants.Select(p => new {
                    userId = p.UserId,
                    name = p.User?.Username,
                    joinTime = p.JoinTime,
                    leaveTime = p.LeaveTime
                }),
                startTime = meeting.StartTime,
                endTime = meeting.EndTime,
                status = meeting.Status,
                isPrivate = meeting.IsPrivate,
                guestCode = meeting.GuestCode
            });
        }

        [HttpGet("scheduled")]
        public async Task<IActionResult> GetScheduledMeetingsByMonth([FromQuery] int year, [FromQuery] int month, [FromQuery] string userId) {
            if (year < 2020 || year > 2030 || month < 1 || month > 12)
                return BadRequest("Invalid year or month");

            var meetings = string.IsNullOrEmpty(userId)
                ? await _meetingRepository.GetScheduledMeetingsByMonthAsync(year, month)
                : await _meetingRepository.GetScheduledMeetingsByMonthAsync(year, month, Guid.Parse(userId));

            return Ok(meetings.Select(m => new {
                id = m.Id,
                title = m.Title,
                description = m.Description,
                hostId = m.HostId,
                hostName = m.Host?.Username,
                startTime = m.StartTime,
                endTime = m.EndTime,
                status = m.Status,
                isDeleted = m.IsDeleted
            }));
        }

        [HttpGet("scheduled/date")]
        public async Task<IActionResult> GetScheduledMeetingsByDate([FromQuery] DateTime date, [FromQuery] string userId) {
            DateTime utcDate = DateTime.SpecifyKind(date, DateTimeKind.Utc);

            var meetings = string.IsNullOrEmpty(userId)
                ? await _meetingRepository.GetScheduledMeetingsByDateAsync(utcDate)
                : await _meetingRepository.GetScheduledMeetingsByDateAsync(utcDate, Guid.Parse(userId));

            return Ok(meetings.Select(m => new {
                id = m.Id,
                title = m.Title,
                description = m.Description,
                hostId = m.HostId,
                hostName = m.Host?.Username,
                startTime = m.StartTime,
                endTime = m.EndTime,
                status = m.Status
            }));
        }

        [HttpGet("recordings/{userId}")]
        public async Task<IActionResult> GetUserRecordings(string userId) {
            if (string.IsNullOrEmpty(userId))
                return BadRequest("User ID is required");

            var recordings = await _meetingRepository.GetRecordingsByUserIdAsync(Guid.Parse(userId));

            return Ok(recordings.Select(r => new {
                id = r.Id,
                meetingId = r.MeetingId,
                meetingTitle = r.Meeting?.Title,
                storagePath = r.StoragePath,
                duration = r.Duration,
                processed = r.Processed,
                transcription = r.Transcription,
                createdAt = r.CreatedAt
            }));
        }

        [HttpPost("{id}/join")]
        public async Task<IActionResult> JoinMeeting(Guid id, [FromBody] JoinMeetingDto joinDto) {
            var meeting = await _meetingRepository.GetMeetingByIdAsync(id);
            if (meeting == null)
                return NotFound();

            var userId = joinDto.UserId;
            if (string.IsNullOrEmpty(userId))
                return BadRequest("User ID is required");

            if (meeting.IsPrivate && meeting.HostId != Guid.Parse(userId)) {
                if (string.IsNullOrEmpty(joinDto.GuestCode) || joinDto.GuestCode != meeting.GuestCode)
                    return BadRequest("Invalid guest code");
            }

            await _meetingRepository.AddParticipantAsync(id, Guid.Parse(userId), joinDto.DeviceInfo);
            return Ok();
        }

        [HttpPost("{id}/leave")]
        public async Task<IActionResult> LeaveMeeting(Guid id, [FromBody] LeaveMeetingDto leaveDto) {
            var userId = leaveDto.UserId;
            if (string.IsNullOrEmpty(userId))
                return BadRequest("User ID is required");

            await _meetingRepository.RemoveParticipantAsync(id, Guid.Parse(userId));
            return Ok();
        }

        [HttpPost("{id}/recording")]
        public async Task<IActionResult> AddRecording(Guid id, [FromBody] AddRecordingDto recordingDto) {
            var meeting = await _meetingRepository.GetMeetingByIdAsync(id);
            if (meeting == null)
                return NotFound();

            var userId = recordingDto.UserId;
            if (string.IsNullOrEmpty(userId))
                return BadRequest("User ID is required");

            if (meeting.HostId != Guid.Parse(userId))
                return BadRequest("Only the host can add recordings");

            var recording = new MeetingRecording {
                Id = Guid.NewGuid(),
                MeetingId = id,
                StoragePath = recordingDto.StoragePath,
                Duration = recordingDto.Duration,
                Processed = false,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _meetingRepository.AddRecordingAsync(recording);
            return Ok(new {
                id = result.Id,
                meetingId = result.MeetingId,
                storagePath = result.StoragePath,
                duration = result.Duration,
                createdAt = result.CreatedAt
            });
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMeeting(Guid id, [FromQuery] string userId) {
            if (string.IsNullOrEmpty(userId))
                return BadRequest("User ID is required");

            var meeting = await _meetingRepository.GetMeetingByIdAsync(id);
            if (meeting == null)
                return NotFound("Meeting not found");

            if (meeting.HostId != Guid.Parse(userId))
                return Forbid("Only the host can delete this meeting");

            var result = await _meetingRepository.DeleteMeetingAsync(id);
            if (!result)
                return BadRequest("Failed to delete meeting");

            return Ok(new { message = "Meeting deleted successfully" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMeeting(Guid id, [FromBody] MeetingUpdateDto updateDto) {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var meeting = await _meetingRepository.GetMeetingByIdAsync(id);
            if (meeting == null)
                return NotFound("Meeting not found");

            var userId = updateDto.UserId;
            if (string.IsNullOrEmpty(userId))
                return BadRequest("User ID is required");

            if (meeting.HostId != Guid.Parse(userId))
                return Forbid("Only the host can update this meeting");

            if (!string.IsNullOrEmpty(updateDto.Title))
                meeting.Title = updateDto.Title;

            if (!string.IsNullOrEmpty(updateDto.Description))
                meeting.Description = updateDto.Description;

            if (updateDto.StartTime.HasValue)
                meeting.StartTime = DateTime.SpecifyKind(updateDto.StartTime.Value, DateTimeKind.Utc);

            if (updateDto.EndTime.HasValue)
                meeting.EndTime = DateTime.SpecifyKind(updateDto.EndTime.Value, DateTimeKind.Utc);

            if (!string.IsNullOrEmpty(updateDto.Status))
                meeting.Status = updateDto.Status;

            if (updateDto.MaxParticipants.HasValue)
                meeting.MaxParticipants = updateDto.MaxParticipants.Value;

            var result = await _meetingRepository.UpdateMeetingAsync(meeting);

            return Ok(new {
                id = result.Id,
                title = result.Title,
                description = result.Description,
                startTime = result.StartTime,
                endTime = result.EndTime,
                status = result.Status,
                maxParticipants = result.MaxParticipants,
                isPrivate = result.IsPrivate,
                guestCode = result.GuestCode
            });
        }

        [HttpPost("{id}/send-email")]
        public async Task<IActionResult> SendMeetingEmail(Guid id, [FromBody] SendMeetingEmailDto emailDto) {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var meeting = await _meetingRepository.GetMeetingByIdAsync(id);
            if (meeting == null)
                return NotFound("Meeting not found");

            try {
                var success = await _emailService.SendMeetingScheduleEmailAsync(
                    meeting,
                    emailDto.RecipientEmails,
                    emailDto.SenderName,
                    emailDto.CustomMessage
                );

                if (success) {
                    return Ok(new {
                        message = $"Meeting invitation emails sent successfully to {emailDto.RecipientEmails.Count} recipient(s)",
                        meetingId = meeting.Id,
                        meetingTitle = meeting.Title,
                        recipientCount = emailDto.RecipientEmails.Count,
                        recipients = emailDto.RecipientEmails
                    });
                }
                else {
                    return StatusCode(500, new { message = "Failed to send some or all emails. Please check email configuration." });
                }
            }
            catch (Exception ex) {
                return StatusCode(500, new { message = "Error sending emails", error = ex.Message });
            }
        }

        private static string GenerateRandomCode() {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        [HttpPost("invitation")]
        public async Task<IActionResult> AddInvitation([FromBody] MeetingInvitationDto dto) {
            foreach (var email in dto.Email) {
                var invitation = new MeetingInvitation {
                    Id = Guid.NewGuid(),
                    MeetingId = dto.MeetingId,
                    Email = email,
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow
                };
                await _meetingRepository.AddMeetingInvitationAsync(invitation);
            }

            return Ok();
        }

        [HttpGet("MeetingActive")]
        public async Task<IActionResult> GetActiveMeeting() {
            var rs = await _meetingRepository.CountActiveMeetingsAsync();
            return Ok(rs);
        }

        [HttpGet("MeetingScheduled")]
        public async Task<IActionResult> GetScheduledMeeting() {
            var rs = await _meetingRepository.CountScheduleMeetingAsync();
            return Ok(rs);
        }

        [HttpGet("MeetingRecord")]
        public async Task<IActionResult> GetMeetingRecord() {
            var rs = await _meetingRepository.CountRecordMeetingAsync();
            return Ok(rs);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMeetings() {
            var meetings = await _meetingRepository.GetAll();
            return Ok(meetings);
        }
    }
}
