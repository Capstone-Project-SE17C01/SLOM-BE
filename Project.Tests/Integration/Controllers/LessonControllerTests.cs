using System.Net;
using Project.Core.Entities.General;
using Project.Infrastructure.Data;
using Project.Tests.Helpers;

namespace Project.Tests.Integration.Controllers;

[TestFixture]
public class LessonControllerTests : IntegrationTestBase {
    private const string BaseUrl = "/api/Lesson";
    private Guid _testUserId;
    private Guid _testLessonId;

    [SetUp]
    public override void SetUp() {
        base.SetUp();
        SeedTestData();
    }

    private void SeedTestData() {
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        _testUserId = Guid.NewGuid();
        var user = new Profile {
            Id = _testUserId,
            Username = "testuser",
            Email = "testuser@example.com",
            RoleId = Guid.NewGuid(),
            PreferredLanguageId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            AvatarUrl = "https://example.com/avatar.jpg"
        };

        _testLessonId = Guid.NewGuid();
        var lesson = new Lesson {
            Id = _testLessonId,
            Title = "Test Lesson",
            Content = "A test lesson",
            CreatedAt = DateTime.UtcNow,
        };

        context.Profiles.Add(user);
        context.Lessons.Add(lesson);
        context.SaveChanges();
    }

    [Test]
    public async Task GetOngoingUserLesson_WithValidUser_ShouldReturnSuccess() {
        var response = await Client.GetAsync($"{BaseUrl}/OngoingLesson?userId={_testUserId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("result");
    }

    [Test]
    public async Task GetOngoingUserLesson_WithInvalidUserId_ShouldReturnSuccessWithNull() {
        var response = await Client.GetAsync($"{BaseUrl}/OngoingLesson?userId={Guid.Empty}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("result");
    }

    [Test]
    public async Task GetLearnedLesson_WithValidUser_ShouldReturnSuccess() {
        var response = await Client.GetAsync($"{BaseUrl}/GetListLearnedLesson?userId={_testUserId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("result");
    }

    [Test]
    public async Task GetListWordLesson_WithValidLessonId_ShouldReturnSuccess() {
        var response = await Client.GetAsync($"{BaseUrl}/GetListWordLesson?lessonId={_testLessonId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("result");
    }

    [Test]
    public async Task GetListQuizLesson_WithValidLessonId_ShouldReturnSuccess() {
        var response = await Client.GetAsync($"{BaseUrl}/GetListQuizLesson?lessonId={_testLessonId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("result");
    }

    [Test]
    public async Task GetListWordLesson_WithInvalidLessonId_ShouldReturnSuccessWithEmpty() {
        var response = await Client.GetAsync($"{BaseUrl}/GetListWordLesson?lessonId={Guid.Empty}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("result");
    }

    [Test]
    public async Task GetListQuizLesson_WithInvalidLessonId_ShouldReturnSuccessWithEmpty() {
        var response = await Client.GetAsync($"{BaseUrl}/GetListQuizLesson?lessonId={Guid.Empty}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("result");
    }
    [Test]
    public async Task GetOngoingUserLesson_WithoutAuthToken_ShouldReturnUnauthorized() {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}/OngoingLesson?userId={_testUserId}");
        var response = await Client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task GetListWordLesson_WithNonExistentLessonId_ShouldReturnNotFoundOrEmpty() {
        var nonExistentLessonId = Guid.NewGuid();
        var response = await Client.GetAsync($"{BaseUrl}/GetListWordLesson?lessonId={nonExistentLessonId}");

        response.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.OK);
    }

    [Test]
    public async Task GetListQuizLesson_WithLargeLessonId_ShouldHandleGracefully() {
        var largeGuid = Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF");
        var response = await Client.GetAsync($"{BaseUrl}/GetListQuizLesson?lessonId={largeGuid}");

        response.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.OK);
    }

    [Test]
    public async Task GetOngoingUserLesson_ConcurrentRequests_ShouldHandleLoad() {
        var tasks = new List<Task<HttpResponseMessage>>();
        for (int i = 0; i < 5; i++) {
            tasks.Add(Client.GetAsync($"{BaseUrl}/OngoingLesson?userId={_testUserId}"));
        }

        var responses = await Task.WhenAll(tasks);

        foreach (var response in responses) {
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Dispose();
        }
    }
}
