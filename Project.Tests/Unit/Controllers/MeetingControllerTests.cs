using Microsoft.AspNetCore.Mvc;
using Project.API.Controllers;
using Project.Core.Entities.Business.DTOs.MeetingDTOs;
using Project.Core.Entities.General;
using Project.Core.Interfaces.IRepositories;
using Project.Core.Interfaces.IServices;
using Xunit;

namespace Project.Tests.Unit.Controllers;

public class MeetingControllerTests {
    private readonly Mock<IMeetingRepository> _mockMeetingRepository = new();
    private readonly Mock<IEmailService> _mockEmailService = new();

    private MeetingController CreateController() {
        return new MeetingController(_mockMeetingRepository.Object, _mockEmailService.Object);
    }

    #region CreateMeeting Tests

    [Fact]
    public async Task CreateMeeting_WithValidImmediateMeeting_ReturnsOkWithMeetingData() {
        // Arrange
        var controller = CreateController();
        var meetingDto = new MeetingCreateDto {
            Title = "Test Meeting",
            Description = "Test Description",
            IsImmediate = true,
            Duration = 60,
            IsPrivate = false,
            UserId = Guid.NewGuid().ToString()
        };
        var createdMeeting = new Meeting {
            Id = Guid.NewGuid(),
            HostId = Guid.Parse(meetingDto.UserId),
            Title = meetingDto.Title,
            Description = meetingDto.Description,
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddMinutes(60),
            Status = "Active",
            IsPrivate = false,
            GuestCode = null,
            CreatedAt = DateTime.UtcNow
        };

        _mockMeetingRepository.Setup(x => x.CreateMeetingAsync(It.IsAny<Meeting>())).ReturnsAsync(createdMeeting);

        // Act
        var result = await controller.CreateMeeting(meetingDto);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().NotBeNull();

        // Use reflection to access anonymous object properties
        var value = okResult.Value;
        value.Should().NotBeNull();
        var idProperty = value!.GetType().GetProperty("id");
        var titleProperty = value!.GetType().GetProperty("title");
        var statusProperty = value!.GetType().GetProperty("status");

        idProperty.Should().NotBeNull();
        titleProperty.Should().NotBeNull();
        statusProperty.Should().NotBeNull();

        idProperty!.GetValue(value).Should().Be(createdMeeting.Id);
        titleProperty!.GetValue(value).Should().Be("Test Meeting");
        statusProperty!.GetValue(value).Should().Be("Active");
    }

    [Fact]
    public async Task CreateMeeting_WithValidScheduledMeeting_ReturnsOkWithMeetingData() {
        // Arrange
        var controller = CreateController();
        var startTime = DateTime.UtcNow.AddHours(1);
        var meetingDto = new MeetingCreateDto {
            Title = "Scheduled Meeting",
            Description = "Scheduled Description",
            IsImmediate = false,
            StartTime = startTime,
            Duration = 90,
            IsPrivate = true,
            UserId = Guid.NewGuid().ToString()
        };
        var createdMeeting = new Meeting {
            Id = Guid.NewGuid(),
            HostId = Guid.Parse(meetingDto.UserId),
            Title = meetingDto.Title,
            Description = meetingDto.Description,
            StartTime = startTime,
            EndTime = startTime.AddMinutes(90),
            Status = "Scheduled",
            IsPrivate = true,
            GuestCode = "ABC123",
            CreatedAt = DateTime.UtcNow
        };

        _mockMeetingRepository.Setup(x => x.CreateMeetingAsync(It.IsAny<Meeting>())).ReturnsAsync(createdMeeting);

        // Act
        var result = await controller.CreateMeeting(meetingDto);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().NotBeNull();

        var value = okResult.Value;
        value.Should().NotBeNull();
        var titleProperty = value!.GetType().GetProperty("title");
        var statusProperty = value!.GetType().GetProperty("status");
        var isPrivateProperty = value!.GetType().GetProperty("isPrivate");

        titleProperty.Should().NotBeNull();
        statusProperty.Should().NotBeNull();
        isPrivateProperty.Should().NotBeNull();
        titleProperty!.GetValue(value).Should().Be("Scheduled Meeting");
        statusProperty!.GetValue(value).Should().Be("Scheduled");
        isPrivateProperty!.GetValue(value).Should().Be(true);
    }

    [Fact]
    public async Task CreateMeeting_WithEmptyUserId_ReturnsBadRequest() {
        // Arrange
        var controller = CreateController();
        var meetingDto = new MeetingCreateDto {
            Title = "Test Meeting",
            Description = "Test Description",
            IsImmediate = true,
            Duration = 60,
            IsPrivate = false,
            UserId = ""
        };

        // Act
        var result = await controller.CreateMeeting(meetingDto);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be("User ID is required");
    }

    [Fact]
    public async Task CreateMeeting_WithNullUserId_ReturnsBadRequest() {
        // Arrange
        var controller = CreateController();
        var meetingDto = new MeetingCreateDto {
            Title = "Test Meeting",
            Description = "Test Description",
            IsImmediate = true,
            Duration = 60,
            IsPrivate = false,
            UserId = null!
        };

        // Act
        var result = await controller.CreateMeeting(meetingDto);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be("User ID is required");
    }

    #endregion

    #region GetActiveMeetings Tests

    [Fact]
    public async Task GetActiveMeetings_WithValidUserId_ReturnsOkWithMeetings() {
        // Arrange
        var controller = CreateController();
        var userId = Guid.NewGuid().ToString();
        var meetings = new List<Meeting> {
            new() {
                Id = Guid.NewGuid(),
                Title = "Active Meeting 1",
                Description = "Description 1",
                HostId = Guid.NewGuid(),
                Host = new Profile { Username = "Host1" },
                Participants = new List<MeetingParticipant>(),
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddHours(1),
                Status = "Active",
                IsPrivate = false
            }
        };

        _mockMeetingRepository.Setup(x => x.GetActiveMeetingsAsync(Guid.Parse(userId))).ReturnsAsync(meetings);

        // Act
        var result = await controller.GetActiveMeetings(userId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().NotBeNull();

        var value = okResult.Value;
        var enumerable = value as IEnumerable<object>;
        enumerable.Should().NotBeNull();
        enumerable!.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetActiveMeetings_WithEmptyUserId_ReturnsOkWithAllMeetings() {
        // Arrange
        var controller = CreateController();
        var meetings = new List<Meeting> {
            new() {
                Id = Guid.NewGuid(),
                Title = "Active Meeting 1",
                Description = "Description 1",
                HostId = Guid.NewGuid(),
                Host = new Profile { Username = "Host1" },
                Participants = new List<MeetingParticipant>(),
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddHours(1),
                Status = "Active",
                IsPrivate = false
            }
        };

        _mockMeetingRepository.Setup(x => x.GetActiveMeetingsAsync()).ReturnsAsync(meetings);

        // Act
        var result = await controller.GetActiveMeetings("");

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().NotBeNull();

        var value = okResult.Value;
        var enumerable = value as IEnumerable<object>;
        enumerable.Should().NotBeNull();
        enumerable!.Should().HaveCount(1);
    }

    #endregion

    #region GetMeeting Tests

    [Fact]
    public async Task GetMeeting_WithValidId_ReturnsOkWithMeetingData() {
        // Arrange
        var controller = CreateController();
        var meetingId = Guid.NewGuid();
        var meeting = new Meeting {
            Id = meetingId,
            Title = "Test Meeting",
            Description = "Test Description",
            HostId = Guid.NewGuid(),
            Host = new Profile { Username = "HostUser" },
            Participants = new List<MeetingParticipant>(),
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddHours(1),
            Status = "Active",
            IsPrivate = false,
            GuestCode = null
        };

        _mockMeetingRepository.Setup(x => x.GetMeetingByIdAsync(meetingId)).ReturnsAsync(meeting);

        // Act
        var result = await controller.GetMeeting(meetingId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().NotBeNull();

        var value = okResult.Value;
        value.Should().NotBeNull();
        var idProperty = value!.GetType().GetProperty("id");
        var titleProperty = value!.GetType().GetProperty("title");

        idProperty.Should().NotBeNull();
        titleProperty.Should().NotBeNull();
        idProperty!.GetValue(value).Should().Be(meetingId);
        titleProperty!.GetValue(value).Should().Be("Test Meeting");
    }

    [Fact]
    public async Task GetMeeting_WithNonExistentId_ReturnsNotFound() {
        // Arrange
        var controller = CreateController();
        var meetingId = Guid.NewGuid();

        _mockMeetingRepository.Setup(x => x.GetMeetingByIdAsync(meetingId)).ReturnsAsync((Meeting?)null);

        // Act
        var result = await controller.GetMeeting(meetingId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    #endregion

    #region GetScheduledMeetingsByMonth Tests

    [Fact]
    public async Task GetScheduledMeetingsByMonth_WithValidParameters_ReturnsOkWithMeetings() {
        // Arrange
        var controller = CreateController();
        var year = 2024;
        var month = 6;
        var userId = Guid.NewGuid().ToString();
        var meetings = new List<Meeting> {
            new() {
                Id = Guid.NewGuid(),
                Title = "Scheduled Meeting",
                Description = "Description",
                HostId = Guid.NewGuid(),
                Host = new Profile { Username = "Host" },
                StartTime = new DateTime(2024, 6, 15, 10, 0, 0),
                EndTime = new DateTime(2024, 6, 15, 11, 0, 0),
                Status = "Scheduled"
            }
        };

        _mockMeetingRepository.Setup(x => x.GetScheduledMeetingsByMonthAsync(year, month, Guid.Parse(userId))).ReturnsAsync(meetings);

        // Act
        var result = await controller.GetScheduledMeetingsByMonth(year, month, userId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().NotBeNull();

        var value = okResult.Value;
        var enumerable = value as IEnumerable<object>;
        enumerable.Should().NotBeNull();
        enumerable!.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetScheduledMeetingsByMonth_WithInvalidYear_ReturnsBadRequest() {
        // Arrange
        var controller = CreateController();
        var year = 2019; // Invalid year
        var month = 6;
        var userId = Guid.NewGuid().ToString();

        // Act
        var result = await controller.GetScheduledMeetingsByMonth(year, month, userId);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be("Invalid year or month");
    }

    [Fact]
    public async Task GetScheduledMeetingsByMonth_WithInvalidMonth_ReturnsBadRequest() {
        // Arrange
        var controller = CreateController();
        var year = 2024;
        var month = 13; // Invalid month
        var userId = Guid.NewGuid().ToString();

        // Act
        var result = await controller.GetScheduledMeetingsByMonth(year, month, userId);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be("Invalid year or month");
    }

    #endregion

    #region JoinMeeting Tests

    [Fact]
    public async Task JoinMeeting_WithValidPublicMeeting_ReturnsOk() {
        // Arrange
        var controller = CreateController();
        var meetingId = Guid.NewGuid();
        var joinDto = new JoinMeetingDto {
            UserId = Guid.NewGuid().ToString(),
            GuestCode = "ABC123",
            DeviceInfo = "Chrome/Windows"
        };
        var meeting = new Meeting {
            Id = meetingId,
            IsPrivate = false,
            HostId = Guid.NewGuid(),
            GuestCode = null
        };

        _mockMeetingRepository.Setup(x => x.GetMeetingByIdAsync(meetingId)).ReturnsAsync(meeting);
        _mockMeetingRepository.Setup(x => x.AddParticipantAsync(meetingId, Guid.Parse(joinDto.UserId), joinDto.DeviceInfo)).ReturnsAsync(true);

        // Act
        var result = await controller.JoinMeeting(meetingId, joinDto);

        // Assert
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task JoinMeeting_WithValidPrivateMeetingAndCorrectCode_ReturnsOk() {
        // Arrange
        var controller = CreateController();
        var meetingId = Guid.NewGuid();
        var userId = Guid.NewGuid().ToString();
        var guestCode = "ABC123";
        var joinDto = new JoinMeetingDto {
            UserId = userId,
            GuestCode = guestCode,
            DeviceInfo = "Chrome/Windows"
        };
        var meeting = new Meeting {
            Id = meetingId,
            IsPrivate = true,
            HostId = Guid.NewGuid(),
            GuestCode = guestCode
        };

        _mockMeetingRepository.Setup(x => x.GetMeetingByIdAsync(meetingId)).ReturnsAsync(meeting);
        _mockMeetingRepository.Setup(x => x.AddParticipantAsync(meetingId, Guid.Parse(userId), joinDto.DeviceInfo)).ReturnsAsync(true);

        // Act
        var result = await controller.JoinMeeting(meetingId, joinDto);

        // Assert
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task JoinMeeting_WithNonExistentMeeting_ReturnsNotFound() {
        // Arrange
        var controller = CreateController();
        var meetingId = Guid.NewGuid();
        var joinDto = new JoinMeetingDto {
            UserId = Guid.NewGuid().ToString(),
            GuestCode = "ABC123",
            DeviceInfo = "Chrome/Windows"
        };

        _mockMeetingRepository.Setup(x => x.GetMeetingByIdAsync(meetingId)).ReturnsAsync((Meeting?)null);

        // Act
        var result = await controller.JoinMeeting(meetingId, joinDto);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task JoinMeeting_WithPrivateMeetingAndIncorrectCode_ReturnsBadRequest() {
        // Arrange
        var controller = CreateController();
        var meetingId = Guid.NewGuid();
        var joinDto = new JoinMeetingDto {
            UserId = Guid.NewGuid().ToString(),
            GuestCode = "WRONG123",
            DeviceInfo = "Chrome/Windows"
        };
        var meeting = new Meeting {
            Id = meetingId,
            IsPrivate = true,
            HostId = Guid.NewGuid(),
            GuestCode = "ABC123"
        };

        _mockMeetingRepository.Setup(x => x.GetMeetingByIdAsync(meetingId)).ReturnsAsync(meeting);

        // Act
        var result = await controller.JoinMeeting(meetingId, joinDto);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be("Invalid guest code");
    }

    [Fact]
    public async Task JoinMeeting_WithEmptyUserId_ReturnsBadRequest() {
        // Arrange
        var controller = CreateController();
        var meetingId = Guid.NewGuid();
        var joinDto = new JoinMeetingDto {
            UserId = "",
            GuestCode = "ABC123",
            DeviceInfo = "Chrome/Windows"
        };
        var meeting = new Meeting {
            Id = meetingId,
            IsPrivate = false,
            HostId = Guid.NewGuid()
        };

        _mockMeetingRepository.Setup(x => x.GetMeetingByIdAsync(meetingId)).ReturnsAsync(meeting);

        // Act
        var result = await controller.JoinMeeting(meetingId, joinDto);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be("User ID is required");
    }

    #endregion

    #region LeaveMeeting Tests

    [Fact]
    public async Task LeaveMeeting_WithValidParameters_ReturnsOk() {
        // Arrange
        var controller = CreateController();
        var meetingId = Guid.NewGuid();
        var leaveDto = new LeaveMeetingDto {
            UserId = Guid.NewGuid().ToString()
        };

        _mockMeetingRepository.Setup(x => x.RemoveParticipantAsync(meetingId, Guid.Parse(leaveDto.UserId))).ReturnsAsync(true);

        // Act
        var result = await controller.LeaveMeeting(meetingId, leaveDto);

        // Assert
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task LeaveMeeting_WithEmptyUserId_ReturnsBadRequest() {
        // Arrange
        var controller = CreateController();
        var meetingId = Guid.NewGuid();
        var leaveDto = new LeaveMeetingDto {
            UserId = ""
        };

        // Act
        var result = await controller.LeaveMeeting(meetingId, leaveDto);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be("User ID is required");
    }

    #endregion

    #region AddRecording Tests

    [Fact]
    public async Task AddRecording_WithValidHostUser_ReturnsOkWithRecordingData() {
        // Arrange
        var controller = CreateController();
        var meetingId = Guid.NewGuid();
        var userId = Guid.NewGuid().ToString();
        var recordingDto = new AddRecordingDto {
            StoragePath = "/recordings/meeting1.mp4",
            Duration = 3600,
            UserId = userId
        };
        var meeting = new Meeting {
            Id = meetingId,
            HostId = Guid.Parse(userId)
        };
        var recording = new MeetingRecording {
            Id = Guid.NewGuid(),
            MeetingId = meetingId,
            StoragePath = recordingDto.StoragePath,
            Duration = recordingDto.Duration,
            Processed = false,
            CreatedAt = DateTime.UtcNow
        };

        _mockMeetingRepository.Setup(x => x.GetMeetingByIdAsync(meetingId)).ReturnsAsync(meeting);
        _mockMeetingRepository.Setup(x => x.AddRecordingAsync(It.IsAny<MeetingRecording>())).ReturnsAsync(recording);

        // Act
        var result = await controller.AddRecording(meetingId, recordingDto);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().NotBeNull();

        var value = okResult.Value;
        value.Should().NotBeNull();
        var meetingIdProperty = value!.GetType().GetProperty("meetingId");
        var storagePathProperty = value!.GetType().GetProperty("storagePath");

        meetingIdProperty.Should().NotBeNull();
        storagePathProperty.Should().NotBeNull();
        meetingIdProperty!.GetValue(value).Should().Be(meetingId);
        storagePathProperty!.GetValue(value).Should().Be("/recordings/meeting1.mp4");
    }

    [Fact]
    public async Task AddRecording_WithNonExistentMeeting_ReturnsNotFound() {
        // Arrange
        var controller = CreateController();
        var meetingId = Guid.NewGuid();
        var recordingDto = new AddRecordingDto {
            StoragePath = "/recordings/meeting1.mp4",
            Duration = 3600,
            UserId = Guid.NewGuid().ToString()
        };

        _mockMeetingRepository.Setup(x => x.GetMeetingByIdAsync(meetingId)).ReturnsAsync((Meeting?)null);

        // Act
        var result = await controller.AddRecording(meetingId, recordingDto);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task AddRecording_WithNonHostUser_ReturnsBadRequest() {
        // Arrange
        var controller = CreateController();
        var meetingId = Guid.NewGuid();
        var hostId = Guid.NewGuid();
        var nonHostId = Guid.NewGuid();
        var recordingDto = new AddRecordingDto {
            StoragePath = "/recordings/meeting1.mp4",
            Duration = 3600,
            UserId = nonHostId.ToString()
        };
        var meeting = new Meeting {
            Id = meetingId,
            HostId = hostId
        };

        _mockMeetingRepository.Setup(x => x.GetMeetingByIdAsync(meetingId)).ReturnsAsync(meeting);

        // Act
        var result = await controller.AddRecording(meetingId, recordingDto);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be("Only the host can add recordings");
    }

    [Fact]
    public async Task AddRecording_WithEmptyUserId_ReturnsBadRequest() {
        // Arrange
        var controller = CreateController();
        var meetingId = Guid.NewGuid();
        var recordingDto = new AddRecordingDto {
            StoragePath = "/recordings/meeting1.mp4",
            Duration = 3600,
            UserId = ""
        };
        var meeting = new Meeting {
            Id = meetingId,
            HostId = Guid.NewGuid()
        };

        _mockMeetingRepository.Setup(x => x.GetMeetingByIdAsync(meetingId)).ReturnsAsync(meeting);

        // Act
        var result = await controller.AddRecording(meetingId, recordingDto);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be("User ID is required");
    }

    #endregion

    #region DeleteMeeting Tests

    [Fact]
    public async Task DeleteMeeting_WithValidHostUser_ReturnsOk() {
        // Arrange
        var controller = CreateController();
        var meetingId = Guid.NewGuid();
        var userId = Guid.NewGuid().ToString();
        var meeting = new Meeting {
            Id = meetingId,
            HostId = Guid.Parse(userId)
        };

        _mockMeetingRepository.Setup(x => x.GetMeetingByIdAsync(meetingId)).ReturnsAsync(meeting);
        _mockMeetingRepository.Setup(x => x.DeleteMeetingAsync(meetingId)).ReturnsAsync(true);

        // Act
        var result = await controller.DeleteMeeting(meetingId, userId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().NotBeNull();

        var value = okResult.Value;
        value.Should().NotBeNull();
        var messageProperty = value!.GetType().GetProperty("message");
        messageProperty.Should().NotBeNull();
        messageProperty!.GetValue(value).Should().Be("Meeting deleted successfully");
    }

    [Fact]
    public async Task DeleteMeeting_WithNonExistentMeeting_ReturnsNotFound() {
        // Arrange
        var controller = CreateController();
        var meetingId = Guid.NewGuid();
        var userId = Guid.NewGuid().ToString();

        _mockMeetingRepository.Setup(x => x.GetMeetingByIdAsync(meetingId)).ReturnsAsync((Meeting?)null);

        // Act
        var result = await controller.DeleteMeeting(meetingId, userId);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult!.Value.Should().Be("Meeting not found");
    }

    [Fact]
    public async Task DeleteMeeting_WithNonHostUser_ReturnsForbid() {
        // Arrange
        var controller = CreateController();
        var meetingId = Guid.NewGuid();
        var hostId = Guid.NewGuid();
        var nonHostId = Guid.NewGuid().ToString();
        var meeting = new Meeting {
            Id = meetingId,
            HostId = hostId
        };

        _mockMeetingRepository.Setup(x => x.GetMeetingByIdAsync(meetingId)).ReturnsAsync(meeting);

        // Act
        var result = await controller.DeleteMeeting(meetingId, nonHostId);

        // Assert
        result.Should().BeOfType<ForbidResult>();
    }

    [Fact]
    public async Task DeleteMeeting_WithEmptyUserId_ReturnsBadRequest() {
        // Arrange
        var controller = CreateController();
        var meetingId = Guid.NewGuid();

        // Act
        var result = await controller.DeleteMeeting(meetingId, "");

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be("User ID is required");
    }

    [Fact]
    public async Task DeleteMeeting_WithFailedDeletion_ReturnsBadRequest() {
        // Arrange
        var controller = CreateController();
        var meetingId = Guid.NewGuid();
        var userId = Guid.NewGuid().ToString();
        var meeting = new Meeting {
            Id = meetingId,
            HostId = Guid.Parse(userId)
        };

        _mockMeetingRepository.Setup(x => x.GetMeetingByIdAsync(meetingId)).ReturnsAsync(meeting);
        _mockMeetingRepository.Setup(x => x.DeleteMeetingAsync(meetingId)).ReturnsAsync(false);

        // Act
        var result = await controller.DeleteMeeting(meetingId, userId);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be("Failed to delete meeting");
    }

    #endregion

    #region SendMeetingEmail Tests

    [Fact]
    public async Task SendMeetingEmail_WithValidParametersAndSuccess_ReturnsOk() {
        // Arrange
        var controller = CreateController();
        var meetingId = Guid.NewGuid();
        var emailDto = new SendMeetingEmailDto {
            RecipientEmails = new List<string> { "user1@example.com", "user2@example.com" },
            SenderName = "Test Sender",
            CustomMessage = "Please join the meeting"
        };
        var meeting = new Meeting {
            Id = meetingId,
            Title = "Test Meeting"
        };

        _mockMeetingRepository.Setup(x => x.GetMeetingByIdAsync(meetingId)).ReturnsAsync(meeting);
        _mockEmailService.Setup(x => x.SendMeetingScheduleEmailAsync(meeting, emailDto.RecipientEmails, emailDto.SenderName, emailDto.CustomMessage)).ReturnsAsync(true);

        // Act
        var result = await controller.SendMeetingEmail(meetingId, emailDto);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().NotBeNull();

        var value = okResult.Value;
        value.Should().NotBeNull();
        var messageProperty = value!.GetType().GetProperty("message");
        var recipientCountProperty = value!.GetType().GetProperty("recipientCount");

        messageProperty.Should().NotBeNull();
        recipientCountProperty.Should().NotBeNull();
        messageProperty!.GetValue(value).Should().Be("Meeting invitation emails sent successfully to 2 recipient(s)");
        recipientCountProperty!.GetValue(value).Should().Be(2);
    }

    [Fact]
    public async Task SendMeetingEmail_WithEmailServiceFailure_ReturnsInternalServerError() {
        // Arrange
        var controller = CreateController();
        var meetingId = Guid.NewGuid();
        var emailDto = new SendMeetingEmailDto {
            RecipientEmails = new List<string> { "user1@example.com" },
            SenderName = "Test Sender",
            CustomMessage = "Please join the meeting"
        };
        var meeting = new Meeting {
            Id = meetingId,
            Title = "Test Meeting"
        };

        _mockMeetingRepository.Setup(x => x.GetMeetingByIdAsync(meetingId)).ReturnsAsync(meeting);
        _mockEmailService.Setup(x => x.SendMeetingScheduleEmailAsync(meeting, emailDto.RecipientEmails, emailDto.SenderName, emailDto.CustomMessage)).ReturnsAsync(false);

        // Act
        var result = await controller.SendMeetingEmail(meetingId, emailDto);

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be(500);

        var value = objectResult.Value;
        value.Should().NotBeNull();
        var messageProperty = value!.GetType().GetProperty("message");
        messageProperty.Should().NotBeNull();
        messageProperty!.GetValue(value).Should().Be("Failed to send some or all emails. Please check email configuration.");
    }

    [Fact]
    public async Task SendMeetingEmail_WithEmailServiceException_ReturnsInternalServerError() {
        // Arrange
        var controller = CreateController();
        var meetingId = Guid.NewGuid();
        var emailDto = new SendMeetingEmailDto {
            RecipientEmails = new List<string> { "user1@example.com" },
            SenderName = "Test Sender",
            CustomMessage = "Please join the meeting"
        };
        var meeting = new Meeting {
            Id = meetingId,
            Title = "Test Meeting"
        };

        _mockMeetingRepository.Setup(x => x.GetMeetingByIdAsync(meetingId)).ReturnsAsync(meeting);
        _mockEmailService.Setup(x => x.SendMeetingScheduleEmailAsync(meeting, emailDto.RecipientEmails, emailDto.SenderName, emailDto.CustomMessage))
            .ThrowsAsync(new Exception("Email service error"));

        // Act
        var result = await controller.SendMeetingEmail(meetingId, emailDto);

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be(500);

        var value = objectResult.Value;
        value.Should().NotBeNull();
        var messageProperty = value!.GetType().GetProperty("message");
        var errorProperty = value!.GetType().GetProperty("error");

        messageProperty.Should().NotBeNull();
        errorProperty.Should().NotBeNull();
        messageProperty!.GetValue(value).Should().Be("Error sending emails");
        errorProperty!.GetValue(value).Should().Be("Email service error");
    }

    #endregion

    #region AddInvitation Tests

    [Fact]
    public async Task AddInvitation_WithValidEmails_ReturnsOk() {
        // Arrange
        var controller = CreateController();
        var invitationDto = new MeetingInvitationDto {
            MeetingId = Guid.NewGuid(),
            Email = new List<string> { "user1@example.com", "user2@example.com" }
        };

        _mockMeetingRepository.Setup(x => x.AddMeetingInvitationAsync(It.IsAny<MeetingInvitation>())).ReturnsAsync(new MeetingInvitation());

        // Act
        var result = await controller.AddInvitation(invitationDto);

        // Assert
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task AddInvitation_WithEmptyEmailList_ReturnsOk() {
        // Arrange
        var controller = CreateController();
        var invitationDto = new MeetingInvitationDto {
            MeetingId = Guid.NewGuid(),
            Email = new List<string>()
        };

        // Act
        var result = await controller.AddInvitation(invitationDto);

        // Assert
        result.Should().BeOfType<OkResult>();
    }

    #endregion

    // Note: MeetingController has proper dependency injection via interfaces for all dependencies,
    // so all functionality can be properly unit tested. The controller has exception handling
    // in the SendMeetingEmail method, so we test exception scenarios for that method.
    // 
    // Note: The controller uses Guid.Parse() for string to Guid conversion, which will throw
    // FormatException for invalid GUIDs, but since the controller doesn't handle these exceptions
    // (except in SendMeetingEmail), we don't test invalid GUID scenarios for other methods.
    // 
    // Note: The GenerateRandomCode() method is private and uses Random, which makes it difficult
    // to test deterministically. Since it's not directly testable and the controller doesn't
    // expose it, we don't test this method directly.
}
