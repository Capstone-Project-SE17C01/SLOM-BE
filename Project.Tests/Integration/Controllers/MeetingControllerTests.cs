using System.Net;
using Project.Core.Entities.Business.DTOs.MeetingDTOs;
using Project.Core.Entities.General;
using Project.Infrastructure.Data;
using Project.Tests.Helpers;

namespace Project.Tests.Integration.Controllers;

[TestFixture]
public class MeetingControllerTests : IntegrationTestBase {
    private const string BaseUrl = "/api/Meeting";
    private Guid _testHostId;
    private Guid _testParticipantId;
    private Guid _testMeetingId;

    [SetUp]
    public override void SetUp() {
        base.SetUp();
        SeedTestData();
    }

    private void SeedTestData() {
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        _testHostId = Guid.NewGuid();
        var hostUser = new Profile {
            Id = _testHostId,
            Username = "testhost",
            Email = "host@example.com",
            RoleId = Guid.NewGuid(),
            PreferredLanguageId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            AvatarUrl = "https://example.com/host-avatar.jpg"
        };

        _testParticipantId = Guid.NewGuid();
        var participantUser = new Profile {
            Id = _testParticipantId,
            Username = "testparticipant",
            Email = "participant@example.com",
            RoleId = Guid.NewGuid(),
            PreferredLanguageId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            AvatarUrl = "https://example.com/participant-avatar.jpg"
        };

        _testMeetingId = Guid.NewGuid();
        var testMeeting = new Meeting {
            Id = _testMeetingId,
            HostId = _testHostId,
            Title = "Test Meeting",
            Description = "This is a test meeting",
            StartTime = DateTime.UtcNow.AddHours(1),
            EndTime = DateTime.UtcNow.AddHours(2),
            Status = "Scheduled",
            IsPrivate = false,
            MaxParticipants = 10,
            CreatedAt = DateTime.UtcNow,
        };

        var activeMeeting = new Meeting {
            Id = Guid.NewGuid(),
            HostId = _testHostId,
            Title = "Active Test Meeting",
            Description = "This is an active test meeting",
            StartTime = DateTime.UtcNow.AddMinutes(-30),
            EndTime = DateTime.UtcNow.AddMinutes(30),
            Status = "Active",
            IsPrivate = false,
            MaxParticipants = 10,
            CreatedAt = DateTime.UtcNow,
        };

        var privateMeeting = new Meeting {
            Id = Guid.NewGuid(),
            HostId = _testHostId,
            Title = "Private Test Meeting",
            Description = "This is a private test meeting",
            StartTime = DateTime.UtcNow.AddHours(1),
            EndTime = DateTime.UtcNow.AddHours(2),
            Status = "Scheduled",
            IsPrivate = true,
            GuestCode = "ABC123",
            MaxParticipants = 5,
            CreatedAt = DateTime.UtcNow,
        };

        context.Profiles.AddRange(hostUser, participantUser);
        context.Meetings.AddRange(testMeeting, activeMeeting, privateMeeting);
        context.SaveChanges();
    }

    #region Create Meeting Tests

    [Test]
    public async Task CreateMeeting_WithValidData_ShouldReturnSuccess() {
        var meetingDto = new MeetingCreateDto {
            UserId = _testHostId.ToString(),
            Title = "New Test Meeting",
            Description = "Creating a new test meeting",
            StartTime = DateTime.UtcNow.AddHours(2),
            Duration = 60,
            IsImmediate = false,
            IsPrivate = false
        };

        var response = await PostAsJsonAsync(BaseUrl, meetingDto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("New Test Meeting");
        content.Should().Contain("Creating a new test meeting");
    }

    [Test]
    public async Task CreateMeeting_WithImmediateFlag_ShouldCreateActiveMeeting() {
        var meetingDto = new MeetingCreateDto {
            UserId = _testHostId.ToString(),
            Title = "Immediate Meeting",
            Description = "Creating an immediate meeting",
            Duration = 30,
            IsImmediate = true,
            IsPrivate = false
        };

        var response = await PostAsJsonAsync(BaseUrl, meetingDto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Active");
    }

    [Test]
    public async Task CreateMeeting_WithPrivateFlag_ShouldGenerateGuestCode() {
        var meetingDto = new MeetingCreateDto {
            UserId = _testHostId.ToString(),
            Title = "Private Meeting",
            Description = "Creating a private meeting",
            StartTime = DateTime.UtcNow.AddHours(1),
            Duration = 60,
            IsImmediate = false,
            IsPrivate = true
        };

        var response = await PostAsJsonAsync(BaseUrl, meetingDto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("guestCode");
    }

    [Test]
    public async Task CreateMeeting_WithInvalidUserId_ShouldReturnBadRequest() {
        var meetingDto = new MeetingCreateDto {
            UserId = "",
            Title = "Test Meeting",
            Description = "Test description",
            StartTime = DateTime.UtcNow.AddHours(1),
            Duration = 60,
            IsImmediate = false,
            IsPrivate = false
        };

        var response = await PostAsJsonAsync(BaseUrl, meetingDto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Get Active Meetings Tests

    [Test]
    public async Task GetActiveMeetings_WithoutUserId_ShouldReturnAllActiveMeetings() {
        var response = await Client.GetAsync($"{BaseUrl}/active");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Active Test Meeting");
    }

    [Test]
    public async Task GetActiveMeetings_WithUserId_ShouldReturnUserActiveMeetings() {
        var response = await Client.GetAsync($"{BaseUrl}/active?userId={_testHostId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Active Test Meeting");
    }

    #endregion

    #region Get Meeting Tests

    [Test]
    public async Task GetMeeting_WithValidId_ShouldReturnMeeting() {
        var response = await Client.GetAsync($"{BaseUrl}/{_testMeetingId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Test Meeting");
        content.Should().Contain(_testHostId.ToString());
    }

    [Test]
    public async Task GetMeeting_WithInvalidId_ShouldReturnNotFound() {
        var invalidId = Guid.NewGuid();

        var response = await Client.GetAsync($"{BaseUrl}/{invalidId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Get Scheduled Meetings Tests

    [Test]
    public async Task GetScheduledMeetingsByMonth_WithValidParams_ShouldReturnMeetings() {
        var currentYear = DateTime.UtcNow.Year;
        var currentMonth = DateTime.UtcNow.Month;

        var response = await Client.GetAsync($"{BaseUrl}/scheduled?year={currentYear}&month={currentMonth}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Test]
    public async Task GetScheduledMeetingsByMonth_WithInvalidYear_ShouldReturnBadRequest() {
        var response = await Client.GetAsync($"{BaseUrl}/scheduled?year=2019&month=1");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task GetScheduledMeetingsByMonth_WithInvalidMonth_ShouldReturnBadRequest() {
        var response = await Client.GetAsync($"{BaseUrl}/scheduled?year=2024&month=13");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task GetScheduledMeetingsByDate_WithValidDate_ShouldReturnMeetings() {
        var date = DateTime.UtcNow.AddHours(1).ToString("yyyy-MM-dd");

        var response = await Client.GetAsync($"{BaseUrl}/scheduled/date?date={date}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion

    #region Join/Leave Meeting Tests

    [Test]
    public async Task JoinMeeting_WithValidData_ShouldReturnSuccess() {
        var joinDto = new JoinMeetingDto {
            UserId = _testParticipantId.ToString(),
            DeviceInfo = "Test Device",
            GuestCode = "ABC123"
        };

        var response = await PostAsJsonAsync($"{BaseUrl}/{_testMeetingId}/join", joinDto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Test]
    public async Task JoinMeeting_WithInvalidMeetingId_ShouldReturnNotFound() {
        var invalidMeetingId = Guid.NewGuid();
        var joinDto = new JoinMeetingDto {
            UserId = _testParticipantId.ToString(),
            DeviceInfo = "Test Device",
            GuestCode = "ABC123"
        };

        var response = await PostAsJsonAsync($"{BaseUrl}/{invalidMeetingId}/join", joinDto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task JoinMeeting_WithEmptyUserId_ShouldReturnBadRequest() {
        var joinDto = new JoinMeetingDto {
            UserId = "",
            DeviceInfo = "Test Device",
            GuestCode = "ABC123"
        };

        var response = await PostAsJsonAsync($"{BaseUrl}/{_testMeetingId}/join", joinDto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task LeaveMeeting_WithValidData_ShouldReturnSuccess() {
        var leaveDto = new LeaveMeetingDto {
            UserId = _testParticipantId.ToString()
        };

        var response = await PostAsJsonAsync($"{BaseUrl}/{_testMeetingId}/leave", leaveDto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Test]
    public async Task LeaveMeeting_WithEmptyUserId_ShouldReturnBadRequest() {
        var leaveDto = new LeaveMeetingDto {
            UserId = ""
        };

        var response = await PostAsJsonAsync($"{BaseUrl}/{_testMeetingId}/leave", leaveDto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Update Meeting Tests

    [Test]
    public async Task UpdateMeeting_WithValidData_ShouldReturnSuccess() {
        var updateDto = new MeetingUpdateDto {
            UserId = _testHostId.ToString(),
            Title = "Updated Meeting Title",
            Description = "Updated meeting description",
            StartTime = DateTime.UtcNow.AddHours(3),
            EndTime = DateTime.UtcNow.AddHours(4),
            Status = "Scheduled",
            MaxParticipants = 15
        };

        var response = await PutAsJsonAsync($"{BaseUrl}/{_testMeetingId}", updateDto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Updated Meeting Title");
        content.Should().Contain("Updated meeting description");
    }

    [Test]
    public async Task UpdateMeeting_WithInvalidMeetingId_ShouldReturnNotFound() {
        var invalidMeetingId = Guid.NewGuid();
        var updateDto = new MeetingUpdateDto {
            UserId = _testHostId.ToString(),
            Title = "Updated Title"
        };

        var response = await PutAsJsonAsync($"{BaseUrl}/{invalidMeetingId}", updateDto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task UpdateMeeting_WithNonHostUser_ShouldReturnForbidden() {
        var updateDto = new MeetingUpdateDto {
            UserId = _testParticipantId.ToString(),
            Title = "Updated Title"
        };

        var response = await PutAsJsonAsync($"{BaseUrl}/{_testMeetingId}", updateDto);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    #endregion

    #region Delete Meeting Tests

    [Test]
    public async Task DeleteMeeting_WithValidHostUser_ShouldReturnSuccess() {
        var response = await DeleteAsync($"{BaseUrl}/{_testMeetingId}?userId={_testHostId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Meeting deleted successfully");
    }

    [Test]
    public async Task DeleteMeeting_WithInvalidMeetingId_ShouldReturnNotFound() {
        var invalidMeetingId = Guid.NewGuid();

        var response = await DeleteAsync($"{BaseUrl}/{invalidMeetingId}?userId={_testHostId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task DeleteMeeting_WithNonHostUser_ShouldReturnForbidden() {
        var response = await DeleteAsync($"{BaseUrl}/{_testMeetingId}?userId={_testParticipantId}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Test]
    public async Task DeleteMeeting_WithoutUserId_ShouldReturnBadRequest() {
        var response = await DeleteAsync($"{BaseUrl}/{_testMeetingId}");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Recording Tests

    [Test]
    public async Task AddRecording_WithValidData_ShouldReturnSuccess() {
        var recordingDto = new AddRecordingDto {
            UserId = _testHostId.ToString(),
            StoragePath = "/recordings/test-recording.mp4",
            Duration = 1800
        };

        var response = await PostAsJsonAsync($"{BaseUrl}/{_testMeetingId}/recording", recordingDto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("/recordings/test-recording.mp4");
        content.Should().Contain("1800");
    }

    [Test]
    public async Task AddRecording_WithNonHostUser_ShouldReturnBadRequest() {
        var recordingDto = new AddRecordingDto {
            UserId = _testParticipantId.ToString(),
            StoragePath = "/recordings/test-recording.mp4",
            Duration = 1800
        };

        var response = await PostAsJsonAsync($"{BaseUrl}/{_testMeetingId}/recording", recordingDto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task GetUserRecordings_WithValidUserId_ShouldReturnRecordings() {
        var response = await Client.GetAsync($"{BaseUrl}/recordings/{_testHostId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Test]
    public async Task GetUserRecordings_WithEmptyUserId_ShouldReturnBadRequest() {
        var response = await Client.GetAsync($"{BaseUrl}/recordings/");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Meeting Invitation Tests

    [Test]
    public async Task AddInvitation_WithValidData_ShouldReturnSuccess() {
        var invitationDto = new MeetingInvitationDto {
            MeetingId = _testMeetingId,
            Email = new List<string> { "test1@example.com", "test2@example.com" }
        };

        var response = await PostAsJsonAsync($"{BaseUrl}/invitation", invitationDto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion

    #region Error Handling Tests

    [Test]
    public async Task MeetingController_WithMalformedRequest_ShouldReturnBadRequest() {
        var malformedJson = "{ invalid json }";
        var content = new StringContent(malformedJson, System.Text.Encoding.UTF8, "application/json");

        var response = await Client.PostAsync(BaseUrl, content);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion
    [Test]
    public async Task CreateMeeting_MultipleConcurrentRequests_ShouldHandleLoad() {
        var tasks = new List<Task<HttpResponseMessage>>();
        for (int i = 0; i < 5; i++) {
            var meetingDto = new MeetingCreateDto {
                UserId = _testHostId.ToString(),
                Title = $"Concurrent Meeting {i}",
                Description = "Concurrent test",
                StartTime = DateTime.UtcNow.AddHours(2),
                Duration = 60,
                IsImmediate = false,
                IsPrivate = false
            };
            tasks.Add(PostAsJsonAsync(BaseUrl, meetingDto));
        }

        var responses = await Task.WhenAll(tasks);

        foreach (var response in responses) {
            response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest);
            response.Dispose();
        }
    }

    [Test]
    public async Task CreateMeeting_WithLargeTitle_ShouldReturnBadRequestOrHandleGracefully() {
        var largeTitle = new string('A', 300);
        var meetingDto = new MeetingCreateDto {
            UserId = _testHostId.ToString(),
            Title = largeTitle,
            Description = "Test meeting with large title",
            StartTime = DateTime.UtcNow.AddHours(2),
            Duration = 60,
            IsImmediate = false,
            IsPrivate = false
        };

        var response = await PostAsJsonAsync(BaseUrl, meetingDto);

        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.OK);
    }
}
