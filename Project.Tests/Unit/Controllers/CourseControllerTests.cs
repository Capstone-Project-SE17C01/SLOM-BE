using Microsoft.AspNetCore.Mvc;
using Project.API.Controllers;
using Project.Core.Entities.Business.DTOs;
using Project.Core.Entities.Business.DTOs.CourseDTOs;
using Project.Core.Entities.General;
using Project.Core.Interfaces.IRepositories;
using Xunit;

namespace Project.Tests.Unit.Controllers;

public class CourseControllerTests {
    private readonly Mock<IProfileRepository> _mockProfileRepository = new();
    private readonly Mock<ICourseRepository> _mockCourseRepository = new();
    private readonly Mock<IModuleRepository> _mockModuleRepository = new();
    private readonly Mock<ILessonRepository> _mockLessonRepository = new();
    private readonly Mock<IQuizRepository> _mockQuizRepository = new();
    private readonly Mock<IUserCourseProgressRepository> _mockUserCourseProgressRepository = new();
    private readonly Mock<IUserModuleProgressRepository> _mockUserModuleProgressRepository = new();
    private readonly Mock<IUserLessonProgressRepository> _mockUserLessonProgressRepository = new();

    private CourseController CreateController() {
        return new CourseController(
            _mockProfileRepository.Object,
            _mockCourseRepository.Object,
            _mockModuleRepository.Object,
            _mockLessonRepository.Object,
            _mockQuizRepository.Object,
            _mockUserCourseProgressRepository.Object,
            _mockUserModuleProgressRepository.Object,
            _mockUserLessonProgressRepository.Object
        );
    }

    #region GetSummary Tests

    [Fact]
    public async Task GetSummary_WithValidData_ReturnsOkWithSummary() {
        // Arrange
        var controller = CreateController();
        var userId = Guid.NewGuid();
        var courseId = Guid.NewGuid();

        _mockModuleRepository.Setup(x => x.CountAsyncByCourseId(courseId)).ReturnsAsync(50);
        _mockLessonRepository.Setup(x => x.CountAsyncByCourseId(courseId)).ReturnsAsync(200);
        _mockQuizRepository.Setup(x => x.CountAsyncByCourseId(courseId)).ReturnsAsync(30);
        _mockUserModuleProgressRepository.Setup(x => x.CountCompletedAsync(courseId, userId)).ReturnsAsync(8);
        _mockUserLessonProgressRepository.Setup(x => x.CountLearnedAsync(courseId, userId)).ReturnsAsync(25);
        _mockUserLessonProgressRepository.Setup(x => x.CountCompletedAsync(courseId, userId)).ReturnsAsync(10);
        _mockUserLessonProgressRepository.Setup(x => x.GetActiveLessonByUserIdAsync(userId)).ReturnsAsync(new Lesson { Id = Guid.NewGuid(), Title = "Active Lesson" });
        _mockUserLessonProgressRepository.Setup(x => x.CountLast7DaysCompletedLessonsAsync(userId)).ReturnsAsync(5);
        _mockUserModuleProgressRepository.Setup(x => x.CountLast7DaysCompletedModulesAsync(userId)).ReturnsAsync(2);
        _mockUserCourseProgressRepository.Setup(x => x.CountLast7DaysCompletedCoursesAsync(userId)).ReturnsAsync(1);

        // Act
        var result = await controller.GetSummary(userId, courseId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeOfType<APIResponse>();
        var apiResponse = okResult.Value as APIResponse;
        apiResponse!.result.Should().BeOfType<SummaryResponseDTO>();
        var summary = apiResponse.result as SummaryResponseDTO;
        summary!.TotalModules.Should().Be(50);
        summary.TotalLessons.Should().Be(200);
        summary.TotalQuizzes.Should().Be(30);
        summary.TotalModulesCompleted.Should().Be(8);
        summary.TotalLessonsLearned.Should().Be(25);
        summary.TotalQuizzesCompleted.Should().Be(10);
        summary.ActiveLesson.Should().NotBeNull();
        summary.Activities.Should().NotBeNull();
        summary.Activities!.RecentLessonsCompleted.Should().Be(5);
        summary.Activities.RecentModulesCompleted.Should().Be(2);
        summary.Activities.RecentCoursesCompleted.Should().Be(1);
    }

    [Fact]
    public async Task GetSummary_WhenRepositoryThrowsException_ReturnsBadRequest() {
        // Arrange
        var controller = CreateController();
        var userId = Guid.NewGuid();
        var courseId = Guid.NewGuid();

        _mockModuleRepository.Setup(x => x.CountAsyncByCourseId(courseId)).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await controller.GetSummary(userId, courseId);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().BeOfType<APIResponse>();
        var apiResponse = badRequestResult.Value as APIResponse;
        apiResponse!.errorMessages.Should().Contain("Invalid request data for Get Summary");
    }

    [Fact]
    public async Task GetSummary_WithEmptyGuids_ReturnsOk() {
        // Arrange
        var controller = CreateController();
        var userId = Guid.Empty;
        var courseId = Guid.Empty;

        _mockModuleRepository.Setup(x => x.CountAsyncByCourseId(courseId)).ReturnsAsync(0);
        _mockLessonRepository.Setup(x => x.CountAsyncByCourseId(courseId)).ReturnsAsync(0);
        _mockQuizRepository.Setup(x => x.CountAsyncByCourseId(courseId)).ReturnsAsync(0);
        _mockUserModuleProgressRepository.Setup(x => x.CountCompletedAsync(courseId, userId)).ReturnsAsync(0);
        _mockUserLessonProgressRepository.Setup(x => x.CountLearnedAsync(courseId, userId)).ReturnsAsync(0);
        _mockUserLessonProgressRepository.Setup(x => x.CountCompletedAsync(courseId, userId)).ReturnsAsync(0);
        _mockUserLessonProgressRepository.Setup(x => x.GetActiveLessonByUserIdAsync(userId)).ReturnsAsync((Lesson?)null);
        _mockUserLessonProgressRepository.Setup(x => x.CountLast7DaysCompletedLessonsAsync(userId)).ReturnsAsync(0);
        _mockUserModuleProgressRepository.Setup(x => x.CountLast7DaysCompletedModulesAsync(userId)).ReturnsAsync(0);
        _mockUserCourseProgressRepository.Setup(x => x.CountLast7DaysCompletedCoursesAsync(userId)).ReturnsAsync(0);

        // Act
        var result = await controller.GetSummary(userId, courseId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        var apiResponse = okResult!.Value as APIResponse;
        var summary = apiResponse!.result as SummaryResponseDTO;
        summary!.TotalModules.Should().Be(0);
        summary.TotalLessons.Should().Be(0);
        summary.TotalQuizzes.Should().Be(0);
        summary.TotalModulesCompleted.Should().Be(0);
        summary.TotalLessonsLearned.Should().Be(0);
        summary.TotalQuizzesCompleted.Should().Be(0);
        summary.ActiveLesson.Should().BeNull();
        summary.Activities.Should().NotBeNull();
        summary.Activities!.RecentLessonsCompleted.Should().Be(0);
        summary.Activities.RecentModulesCompleted.Should().Be(0);
        summary.Activities.RecentCoursesCompleted.Should().Be(0);
    }

    #endregion

    #region GetAllCourses Tests

    [Fact]
    public async Task GetAllCourses_WithValidUserId_ReturnsOkWithCourses() {
        // Arrange
        var controller = CreateController();
        var userId = Guid.NewGuid();
        var allCourses = new List<Course>
        {
            new Course { Id = Guid.NewGuid(), Title = "Course 1", IsPublished = true },
            new Course { Id = Guid.NewGuid(), Title = "Course 2", IsPublished = true },
            new Course { Id = Guid.NewGuid(), Title = "Course 3", IsPublished = false }
        };
        var learningCourses = new List<Course>
        {
            new Course { Id = allCourses[0].Id, Title = "Course 1", IsPublished = true }
        };

        _mockCourseRepository.Setup(x => x.GetAll()).ReturnsAsync(allCourses);
        _mockUserCourseProgressRepository.Setup(x => x.GetCoursesByUserIdAsync(userId)).ReturnsAsync(learningCourses);

        // Act
        var result = await controller.GetAllCourses(userId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeOfType<APIResponse>();
        var apiResponse = okResult.Value as APIResponse;
        apiResponse!.result.Should().BeOfType<ListCourseResponseDTO>();
        var listResponse = apiResponse.result as ListCourseResponseDTO;
        listResponse!.LearningCourses.Should().HaveCount(1);
        listResponse.RemainingCourses.Should().HaveCount(1); // Only Course 2 (Course 3 is not published)
        listResponse.RemainingCourses[0].Title.Should().Be("Course 2");
    }

    [Fact]
    public async Task GetAllCourses_WhenRepositoryThrowsException_ReturnsBadRequest() {
        // Arrange
        var controller = CreateController();
        var userId = Guid.NewGuid();

        _mockCourseRepository.Setup(x => x.GetAll()).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await controller.GetAllCourses(userId);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().BeOfType<APIResponse>();
        var apiResponse = badRequestResult.Value as APIResponse;
        apiResponse!.errorMessages.Should().Contain("Invalid request data for Get All Courses");
    }

    [Fact]
    public async Task GetAllCourses_WithEmptyUserId_ReturnsOk() {
        // Arrange
        var controller = CreateController();
        var userId = Guid.Empty;
        var allCourses = new List<Course>();
        var learningCourses = new List<Course>();

        _mockCourseRepository.Setup(x => x.GetAll()).ReturnsAsync(allCourses);
        _mockUserCourseProgressRepository.Setup(x => x.GetCoursesByUserIdAsync(userId)).ReturnsAsync(learningCourses);

        // Act
        var result = await controller.GetAllCourses(userId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        var apiResponse = okResult!.Value as APIResponse;
        var listResponse = apiResponse!.result as ListCourseResponseDTO;
        listResponse!.LearningCourses.Should().BeEmpty();
        listResponse.RemainingCourses.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllCourses_WithNoLearningCourses_ReturnsAllPublishedCoursesAsRemaining() {
        // Arrange
        var controller = CreateController();
        var userId = Guid.NewGuid();
        var allCourses = new List<Course>
        {
            new Course { Id = Guid.NewGuid(), Title = "Course 1", IsPublished = true },
            new Course { Id = Guid.NewGuid(), Title = "Course 2", IsPublished = true }
        };
        var learningCourses = new List<Course>(); // Empty list

        _mockCourseRepository.Setup(x => x.GetAll()).ReturnsAsync(allCourses);
        _mockUserCourseProgressRepository.Setup(x => x.GetCoursesByUserIdAsync(userId)).ReturnsAsync(learningCourses);

        // Act
        var result = await controller.GetAllCourses(userId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        var apiResponse = okResult!.Value as APIResponse;
        var listResponse = apiResponse!.result as ListCourseResponseDTO;
        listResponse!.LearningCourses.Should().BeEmpty();
        listResponse.RemainingCourses.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAllCourses_WithAllCoursesInLearning_ReturnsEmptyRemainingCourses() {
        // Arrange
        var controller = CreateController();
        var userId = Guid.NewGuid();
        var allCourses = new List<Course>
        {
            new Course { Id = Guid.NewGuid(), Title = "Course 1", IsPublished = true },
            new Course { Id = Guid.NewGuid(), Title = "Course 2", IsPublished = true }
        };

        _mockCourseRepository.Setup(x => x.GetAll()).ReturnsAsync(allCourses);
        _mockUserCourseProgressRepository.Setup(x => x.GetCoursesByUserIdAsync(userId)).ReturnsAsync(allCourses);

        // Act
        var result = await controller.GetAllCourses(userId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        var apiResponse = okResult!.Value as APIResponse;
        var listResponse = apiResponse!.result as ListCourseResponseDTO;
        listResponse!.LearningCourses.Should().HaveCount(2);
        listResponse.RemainingCourses.Should().BeEmpty();
    }

    #endregion

    #region Edge Cases

    [Fact]
    public async Task GetSummary_WithNullActiveLesson_HandlesGracefully() {
        // Arrange
        var controller = CreateController();
        var userId = Guid.NewGuid();
        var courseId = Guid.NewGuid();

        _mockModuleRepository.Setup(x => x.CountAsyncByCourseId(courseId)).ReturnsAsync(50);
        _mockLessonRepository.Setup(x => x.CountAsyncByCourseId(courseId)).ReturnsAsync(200);
        _mockQuizRepository.Setup(x => x.CountAsyncByCourseId(courseId)).ReturnsAsync(30);
        _mockUserModuleProgressRepository.Setup(x => x.CountCompletedAsync(courseId, userId)).ReturnsAsync(8);
        _mockUserLessonProgressRepository.Setup(x => x.CountLearnedAsync(courseId, userId)).ReturnsAsync(25);
        _mockUserLessonProgressRepository.Setup(x => x.CountCompletedAsync(courseId, userId)).ReturnsAsync(10);
        _mockUserLessonProgressRepository.Setup(x => x.GetActiveLessonByUserIdAsync(userId)).ReturnsAsync((Lesson?)null);
        _mockUserLessonProgressRepository.Setup(x => x.CountLast7DaysCompletedLessonsAsync(userId)).ReturnsAsync(5);
        _mockUserModuleProgressRepository.Setup(x => x.CountLast7DaysCompletedModulesAsync(userId)).ReturnsAsync(2);
        _mockUserCourseProgressRepository.Setup(x => x.CountLast7DaysCompletedCoursesAsync(userId)).ReturnsAsync(1);

        // Act
        var result = await controller.GetSummary(userId, courseId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        var apiResponse = okResult!.Value as APIResponse;
        var summary = apiResponse!.result as SummaryResponseDTO;
        summary!.ActiveLesson.Should().BeNull();
    }

    [Fact]
    public async Task GetAllCourses_WithNullCourses_HandlesGracefully() {
        // Arrange
        var controller = CreateController();
        var userId = Guid.NewGuid();

        _mockCourseRepository.Setup(x => x.GetAll()).ReturnsAsync((List<Course>?)null);
        _mockUserCourseProgressRepository.Setup(x => x.GetCoursesByUserIdAsync(userId)).ReturnsAsync(new List<Course>());

        // Act
        var result = await controller.GetAllCourses(userId);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    #endregion

    // Note: CourseController has proper exception handling with try-catch blocks,
    // so we test exception scenarios. All dependencies are injected via interfaces,
    // so all functionality can be properly unit tested.
}
