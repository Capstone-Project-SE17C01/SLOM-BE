using Microsoft.AspNetCore.Mvc;
using Project.Core.Entities.Business.DTOs.MeetingDTOs;
using Project.Core.Entities.General;
using Project.Core.Interfaces.IRepositories;

namespace Project.API.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class MeetingController : ControllerBase {
        private readonly IMeetingRepository _meetingRepository;

        public MeetingController(IMeetingRepository meetingRepository) {
            _meetingRepository = meetingRepository;
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
                isPrivate = m.IsPrivate
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
                status = m.Status
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
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserMeetings(string userId) {
            if (string.IsNullOrEmpty(userId))
                return BadRequest("User ID is required");

            var userGuid = Guid.Parse(userId);
            var meetings = await _meetingRepository.GetUserMeetingsAsync(userGuid);
            var invitedMeetings = await _meetingRepository.GetMeetingsByInvitationAsync(userGuid);

            // Combine both lists and remove duplicates
            var allMeetings = meetings.Union(invitedMeetings, new MeetingComparer()).ToList();

            return Ok(allMeetings.Select(m => new {
                id = m.Id,
                title = m.Title,
                description = m.Description,
                hostId = m.HostId,
                hostName = m.Host?.Username,
                isHost = m.HostId == userGuid,
                isInvited = m.Invitations.Any(i => i.UserId == userGuid),
                invitationStatus = m.Invitations.FirstOrDefault(i => i.UserId == userGuid)?.Status,
                startTime = m.StartTime,
                endTime = m.EndTime,
                status = m.Status,
                isPrivate = m.IsPrivate
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

        [HttpGet("{id}/recordings")]
        public async Task<IActionResult> GetMeetingRecordings(Guid id) {
            var recordings = await _meetingRepository.GetRecordingsForMeetingAsync(id);

            return Ok(recordings.Select(r => new {
                id = r.Id,
                meetingId = r.MeetingId,
                storagePath = r.StoragePath,
                duration = r.Duration,
                processed = r.Processed,
                transcription = r.Transcription,
                createdAt = r.CreatedAt
            }));
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

            // Chỉ host mới có thể update meeting
            if (meeting.HostId != Guid.Parse(userId))
                return Forbid("Only the host can update this meeting");

            // Update meeting properties
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

        private static string GenerateRandomCode() {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        // Meeting Invitation Endpoints
        [HttpPost("schedule-with-invites")]
        public async Task<IActionResult> ScheduleMeetingWithInvites([FromBody] MeetingScheduleWithInvitesDto scheduleDto) {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var hostId = Guid.Parse(scheduleDto.HostId);

            // Create the meeting
            var meeting = new Meeting {
                Id = Guid.NewGuid(),
                HostId = hostId,
                Title = scheduleDto.Title,
                Description = scheduleDto.Description,
                StartTime = DateTime.SpecifyKind(scheduleDto.StartTime, DateTimeKind.Utc),
                EndTime = scheduleDto.EndTime.HasValue ?
                    DateTime.SpecifyKind(scheduleDto.EndTime.Value, DateTimeKind.Utc) :
                    (scheduleDto.Duration.HasValue ?
                        DateTime.SpecifyKind(scheduleDto.StartTime.AddMinutes(scheduleDto.Duration.Value), DateTimeKind.Utc) :
                        null),
                Status = "Scheduled",
                IsPrivate = scheduleDto.IsPrivate,
                MaxParticipants = scheduleDto.MaxParticipants,
                GuestCode = scheduleDto.IsPrivate ? GenerateRandomCode() : null,
                CreatedAt = DateTime.UtcNow
            };

            var createdMeeting = await _meetingRepository.CreateMeetingAsync(meeting);

            // Create invitations
            var invitations = new List<MeetingInvitation>();

            // Invite users by ID
            if (scheduleDto.InviteUserIds != null && scheduleDto.InviteUserIds.Any()) {
                foreach (var userId in scheduleDto.InviteUserIds) {
                    var invitation = new MeetingInvitation {
                        Id = Guid.NewGuid(),
                        MeetingId = createdMeeting.Id,
                        UserId = userId,
                        Status = "Pending",
                        CreatedAt = DateTime.UtcNow,
                        InvitationCode = GenerateInvitationCode()
                    };

                    var createdInvitation = await _meetingRepository.CreateInvitationAsync(invitation);
                    invitations.Add(createdInvitation);
                }
            }

            // Invite users by email
            if (scheduleDto.InviteEmails != null && scheduleDto.InviteEmails.Any()) {
                foreach (var email in scheduleDto.InviteEmails) {
                    var invitation = new MeetingInvitation {
                        Id = Guid.NewGuid(),
                        MeetingId = createdMeeting.Id,
                        Email = email,
                        Status = "Pending",
                        CreatedAt = DateTime.UtcNow,
                        InvitationCode = GenerateInvitationCode()
                    };

                    var createdInvitation = await _meetingRepository.CreateInvitationAsync(invitation);
                    invitations.Add(createdInvitation);
                }
            }

            return Ok(new {
                meeting = new {
                    id = createdMeeting.Id,
                    title = createdMeeting.Title,
                    description = createdMeeting.Description,
                    startTime = createdMeeting.StartTime,
                    endTime = createdMeeting.EndTime,
                    status = createdMeeting.Status,
                    isPrivate = createdMeeting.IsPrivate,
                    guestCode = createdMeeting.GuestCode
                },
                invitations = invitations.Select(i => new {
                    id = i.Id,
                    userId = i.UserId,
                    email = i.Email,
                    status = i.Status,
                    invitationCode = i.InvitationCode,
                    createdAt = i.CreatedAt
                })
            });
        }

        [HttpPost("{meetingId}/invite")]
        public async Task<IActionResult> InviteToMeeting(Guid meetingId, [FromBody] MeetingInviteDto inviteDto) {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var meeting = await _meetingRepository.GetMeetingByIdAsync(meetingId);
            if (meeting == null)
                return NotFound("Meeting not found");

            var hostId = Guid.Parse(inviteDto.HostId);
            if (meeting.HostId != hostId)
                return Forbid("Only the host can invite users to this meeting");

            var invitations = new List<MeetingInvitation>();

            // Invite users by ID
            if (inviteDto.UserIds != null && inviteDto.UserIds.Any()) {
                foreach (var userId in inviteDto.UserIds) {
                    // Check if user is already invited
                    var existingInvitation = (await _meetingRepository.GetInvitationsByMeetingIdAsync(meetingId))
                        .FirstOrDefault(i => i.UserId == userId);

                    if (existingInvitation == null) {
                        var invitation = new MeetingInvitation {
                            Id = Guid.NewGuid(),
                            MeetingId = meetingId,
                            UserId = userId,
                            Status = "Pending",
                            CreatedAt = DateTime.UtcNow,
                            InvitationCode = GenerateInvitationCode()
                        };

                        var createdInvitation = await _meetingRepository.CreateInvitationAsync(invitation);
                        invitations.Add(createdInvitation);
                    }
                }
            }

            // Invite users by email
            if (inviteDto.Emails != null && inviteDto.Emails.Any()) {
                foreach (var email in inviteDto.Emails) {
                    // Check if email is already invited
                    var existingInvitation = (await _meetingRepository.GetInvitationsByMeetingIdAsync(meetingId))
                        .FirstOrDefault(i => i.Email == email);

                    if (existingInvitation == null) {
                        var invitation = new MeetingInvitation {
                            Id = Guid.NewGuid(),
                            MeetingId = meetingId,
                            Email = email,
                            Status = "Pending",
                            CreatedAt = DateTime.UtcNow,
                            InvitationCode = GenerateInvitationCode()
                        };

                        var createdInvitation = await _meetingRepository.CreateInvitationAsync(invitation);
                        invitations.Add(createdInvitation);
                    }
                }
            }

            return Ok(new {
                message = $"Sent {invitations.Count} invitation(s)",
                invitations = invitations.Select(i => new {
                    id = i.Id,
                    userId = i.UserId,
                    email = i.Email,
                    status = i.Status,
                    invitationCode = i.InvitationCode,
                    createdAt = i.CreatedAt
                })
            });
        }

        [HttpGet("invitations/user/{userId}")]
        public async Task<IActionResult> GetUserInvitations(string userId) {
            if (string.IsNullOrEmpty(userId))
                return BadRequest("User ID is required");

            var invitations = await _meetingRepository.GetInvitationsByUserIdAsync(Guid.Parse(userId));

            return Ok(invitations.Select(i => new {
                id = i.Id,
                meetingId = i.MeetingId,
                meetingTitle = i.Meeting?.Title,
                meetingDescription = i.Meeting?.Description,
                hostName = i.Meeting?.Host?.Username,
                startTime = i.Meeting?.StartTime,
                endTime = i.Meeting?.EndTime,
                status = i.Status,
                invitationCode = i.InvitationCode,
                createdAt = i.CreatedAt,
                respondedAt = i.RespondedAt
            }));
        }

        [HttpGet("invitations/email/{email}")]
        public async Task<IActionResult> GetEmailInvitations(string email) {
            if (string.IsNullOrEmpty(email))
                return BadRequest("Email is required");

            var invitations = await _meetingRepository.GetInvitationsByEmailAsync(email);

            return Ok(invitations.Select(i => new {
                id = i.Id,
                meetingId = i.MeetingId,
                meetingTitle = i.Meeting?.Title,
                meetingDescription = i.Meeting?.Description,
                hostName = i.Meeting?.Host?.Username,
                startTime = i.Meeting?.StartTime,
                endTime = i.Meeting?.EndTime,
                status = i.Status,
                invitationCode = i.InvitationCode,
                createdAt = i.CreatedAt,
                respondedAt = i.RespondedAt
            }));
        }

        [HttpPost("invitations/respond")]
        public async Task<IActionResult> RespondToInvitation([FromBody] InvitationResponseDto responseDto) {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var invitation = await _meetingRepository.GetInvitationByCodeAsync(responseDto.InvitationCode);
            if (invitation == null)
                return NotFound("Invitation not found");

            var userId = Guid.Parse(responseDto.UserId);

            // Check if this user can respond to this invitation
            if (invitation.UserId.HasValue && invitation.UserId != userId)
                return Forbid("You are not authorized to respond to this invitation");

            // If invitation was sent by email and user is responding, link the user
            if (!invitation.UserId.HasValue)
                invitation.UserId = userId;

            invitation.Status = responseDto.Response;
            invitation.RespondedAt = DateTime.UtcNow;

            await _meetingRepository.UpdateInvitationAsync(invitation);

            // If accepted, add user as participant
            if (responseDto.Response == "Accepted") {
                await _meetingRepository.AddParticipantAsync(invitation.MeetingId, userId, "Web");
            }

            return Ok(new {
                message = $"Invitation {responseDto.Response.ToLower()}",
                invitation = new {
                    id = invitation.Id,
                    meetingId = invitation.MeetingId,
                    meetingTitle = invitation.Meeting?.Title,
                    status = invitation.Status,
                    respondedAt = invitation.RespondedAt
                }
            });
        }

        [HttpGet("{meetingId}/invitations")]
        public async Task<IActionResult> GetMeetingInvitations(Guid meetingId, [FromQuery] string userId) {
            if (string.IsNullOrEmpty(userId))
                return BadRequest("User ID is required");

            var meeting = await _meetingRepository.GetMeetingByIdAsync(meetingId);
            if (meeting == null)
                return NotFound("Meeting not found");

            var hostId = Guid.Parse(userId);
            if (meeting.HostId != hostId)
                return Forbid("Only the host can view meeting invitations");

            var invitations = await _meetingRepository.GetInvitationsByMeetingIdAsync(meetingId);

            return Ok(invitations.Select(i => new {
                id = i.Id,
                userId = i.UserId,
                userName = i.User?.Username,
                email = i.Email,
                status = i.Status,
                invitationCode = i.InvitationCode,
                createdAt = i.CreatedAt,
                respondedAt = i.RespondedAt
            }));
        }

        [HttpGet("user/{userId}/invited-meetings")]
        public async Task<IActionResult> GetUserInvitedMeetings(string userId) {
            if (string.IsNullOrEmpty(userId))
                return BadRequest("User ID is required");

            var meetings = await _meetingRepository.GetMeetingsByInvitationAsync(Guid.Parse(userId));

            return Ok(meetings.Select(m => new {
                id = m.Id,
                title = m.Title,
                description = m.Description,
                hostId = m.HostId,
                hostName = m.Host?.Username,
                startTime = m.StartTime,
                endTime = m.EndTime,
                status = m.Status,
                isPrivate = m.IsPrivate,
                invitationStatus = m.Invitations.FirstOrDefault(i => i.UserId == Guid.Parse(userId))?.Status
            }));
        }

        private static string GenerateInvitationCode() {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 12)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private class MeetingComparer : IEqualityComparer<Meeting> {
            public bool Equals(Meeting x, Meeting y) {
                return x.Id == y.Id;
            }

            public int GetHashCode(Meeting obj) {
                return obj.Id.GetHashCode();
            }
        }
    }
}
