using System.Net;
using Project.Core.Entities.General;
using Project.Infrastructure.Data;
using Project.Tests.Helpers;

namespace Project.Tests.Integration.Controllers;

[TestFixture]
public class CourseControllerTests : IntegrationTestBase
{
    private const string BaseUrl = "/api/Course";
    private Guid _testUserId;
    private Guid _testCourseId;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        SeedTestData();
    }

    private void SeedTestData()
    {
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        _testUserId = Guid.NewGuid();
        var user = new Profile
        {
            Id = _testUserId,
            Username = "testuser",
            Email = "testuser@example.com",
            RoleId = Guid.NewGuid(),
            PreferredLanguageId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            AvatarUrl = "https://example.com/avatar.jpg"
        };

        _testCourseId = Guid.NewGuid();
        var course = new Course
        {
            Id = _testCourseId,
            Title = "Test Course",
            Description = "A test course",
            IsPublished = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.Profiles.Add(user);
        context.Courses.Add(course);
        context.SaveChanges();
    }

    [Test]
    public async Task GetSummary_WithValidUserAndCourse_ShouldReturnSuccess()
    {
        var response = await Client.GetAsync($"{BaseUrl}/Summary?userId={_testUserId}&courseId={_testCourseId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("TotalCourse");
        content.Should().Contain("TotalModules");
    }

    [Test]
    public async Task GetSummary_WithInvalidUserId_ShouldReturnBadRequest()
    {
        var response = await Client.GetAsync($"{BaseUrl}/Summary?userId={Guid.Empty}&courseId={_testCourseId}");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task GetSummary_WithInvalidCourseId_ShouldReturnBadRequest()
    {
        var response = await Client.GetAsync($"{BaseUrl}/Summary?userId={_testUserId}&courseId={Guid.Empty}");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task GetAllCourses_WithValidUser_ShouldReturnSuccess()
    {
        var response = await Client.GetAsync($"{BaseUrl}/GetListCourses?userId={_testUserId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("LearningCourses");
        content.Should().Contain("RemainingCourses");
    }

    [Test]
    public async Task GetAllCourses_WithInvalidUserId_ShouldReturnBadRequest()
    {
        var response = await Client.GetAsync($"{BaseUrl}/GetListCourses?userId={Guid.Empty}");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    [Test]
    public async Task GetSummary_WithoutAuthToken_ShouldReturnUnauthorized()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}/Summary?userId={_testUserId}&courseId={_testCourseId}");
        var response = await Client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task GetSummary_WithNonExistentCourseId_ShouldReturnNotFound()
    {
        var nonExistentCourseId = Guid.NewGuid();
        var response = await Client.GetAsync($"{BaseUrl}/Summary?userId={_testUserId}&courseId={nonExistentCourseId}");

        response.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task GetAllCourses_WithNonExistentUserId_ShouldReturnNotFound()
    {
        var nonExistentUserId = Guid.NewGuid();
        var response = await Client.GetAsync($"{BaseUrl}/GetListCourses?userId={nonExistentUserId}");

        response.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task GetSummary_WithLargeUserId_ShouldHandleGracefully()
    {
        var largeGuid = Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF");
        var response = await Client.GetAsync($"{BaseUrl}/Summary?userId={largeGuid}&courseId={_testCourseId}");

        response.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task GetAllCourses_ConcurrentRequests_ShouldHandleLoad()
    {
        var tasks = new List<Task<HttpResponseMessage>>();
        for (int i = 0; i < 5; i++)
        {
            tasks.Add(Client.GetAsync($"{BaseUrl}/GetListCourses?userId={_testUserId}"));
        }

        var responses = await Task.WhenAll(tasks);

        foreach (var response in responses)
        {
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Dispose();
        }
    }
}
