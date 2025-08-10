using Microsoft.AspNetCore.Mvc;
using Project.API.Controllers;
using Project.Core.Entities.Business.DTOs;
using Project.Core.Interfaces.IRepositories;
using Xunit;

namespace Project.Tests.Integration.Controllers;

public class LessonProgressControllerTests {
    private readonly Mock<IUserLessonProgressRepository> _mockUserLessonProgressRepository = new();
    private readonly Mock<IUserModuleProgressRepository> _mockUserModuleProgressRepository = new();
    private readonly Mock<IUserCourseProgressRepository> _mockUserCourseProgressRepository = new();
    private readonly Mock<ILessonRepository> _mockLessonRepository = new();

    private LessonProgressController CreateController() {
        return new LessonProgressController(
            _mockUserLessonProgressRepository.Object,
            _mockUserModuleProgressRepository.Object,
            _mockUserCourseProgressRepository.Object,
            _mockLessonRepository.Object
        );
    }

    #region CreateNewUserLesson Tests

    [Fact]
    public async Task CreateNewUserLesson_WithValidData_ReturnsOk() {
        // Arrange
        var controller = CreateController();
        var userId = Guid.NewGuid();
        var lessonId = Guid.NewGuid();

        _mockUserLessonProgressRepository.Setup(x => x.CreateNewLessonProgress(userId, lessonId))
            .ReturnsAsync(true);

        // Act
        var result = await controller.CreateNewUserLesson(userId, lessonId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeOfType<APIResponse>();
        var apiResponse = okResult.Value as APIResponse;
        apiResponse!.errorMessages.Should().BeNull();
        apiResponse.result.Should().Be("Create new lesson progress success");
    }

    [Fact]
    public async Task CreateNewUserLesson_WhenRepositoryReturnsFalse_ReturnsBadRequest() {
        // Arrange
        var controller = CreateController();
        var userId = Guid.NewGuid();
        var lessonId = Guid.NewGuid();

        _mockUserLessonProgressRepository.Setup(x => x.CreateNewLessonProgress(userId, lessonId))
            .ReturnsAsync(false);

        // Act
        var result = await controller.CreateNewUserLesson(userId, lessonId);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().BeOfType<APIResponse>();
        var apiResponse = badRequestResult.Value as APIResponse;
        apiResponse!.errorMessages.Should().Contain("Server Error");
    }

    [Fact]
    public async Task CreateNewUserLesson_WhenRepositoryThrowsException_ReturnsBadRequest() {
        // Arrange
        var controller = CreateController();
        var userId = Guid.NewGuid();
        var lessonId = Guid.NewGuid();

        _mockUserLessonProgressRepository.Setup(x => x.CreateNewLessonProgress(userId, lessonId))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await controller.CreateNewUserLesson(userId, lessonId);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().BeOfType<APIResponse>();
        var apiResponse = badRequestResult.Value as APIResponse;
        apiResponse!.errorMessages.Should().Contain("Database error");
    }

    #endregion

    #region MarkCompleteLesson Tests

    [Fact]
    public async Task MarkCompleteLesson_WithValidData_ReturnsOk() {
        // Arrange
        var controller = CreateController();
        var userId = Guid.NewGuid();
        var lessonId = Guid.NewGuid();

        _mockUserLessonProgressRepository.Setup(x => x.CompleteLessons(userId, lessonId))
            .ReturnsAsync(true);

        // Act
        var result = await controller.MarkCompleteLesson(userId, lessonId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeOfType<APIResponse>();
        var apiResponse = okResult.Value as APIResponse;
        apiResponse!.errorMessages.Should().BeNull();
        apiResponse.result.Should().Be("Mark complete success");
    }

    [Fact]
    public async Task MarkCompleteLesson_WhenRepositoryReturnsFalse_ReturnsBadRequest() {
        // Arrange
        var controller = CreateController();
        var userId = Guid.NewGuid();
        var lessonId = Guid.NewGuid();

        _mockUserLessonProgressRepository.Setup(x => x.CompleteLessons(userId, lessonId))
            .ReturnsAsync(false);

        // Act
        var result = await controller.MarkCompleteLesson(userId, lessonId);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().BeOfType<APIResponse>();
        var apiResponse = badRequestResult.Value as APIResponse;
        apiResponse!.errorMessages.Should().Contain("Server Error");
    }

    [Fact]
    public async Task MarkCompleteLesson_WhenRepositoryThrowsException_ReturnsBadRequest() {
        // Arrange
        var controller = CreateController();
        var userId = Guid.NewGuid();
        var lessonId = Guid.NewGuid();

        _mockUserLessonProgressRepository.Setup(x => x.CompleteLessons(userId, lessonId))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await controller.MarkCompleteLesson(userId, lessonId);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().BeOfType<APIResponse>();
        var apiResponse = badRequestResult.Value as APIResponse;
        apiResponse!.errorMessages.Should().Contain("Database error");
    }

    #endregion

    #region MarkLearnLesson Tests

    [Fact]
    public async Task MarkLearnLesson_WithValidData_ReturnsOk() {
        // Arrange
        var controller = CreateController();
        var userId = Guid.NewGuid();
        var lessonId = Guid.NewGuid();

        _mockUserLessonProgressRepository.Setup(x => x.LearnedLessons(userId, lessonId))
            .ReturnsAsync(true);

        // Act
        var result = await controller.MarkLearnLesson(userId, lessonId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeOfType<APIResponse>();
        var apiResponse = okResult.Value as APIResponse;
        apiResponse!.errorMessages.Should().BeNull();
        apiResponse.result.Should().Be("Mark learned success");
    }

    [Fact]
    public async Task MarkLearnLesson_WhenRepositoryReturnsFalse_ReturnsBadRequest() {
        // Arrange
        var controller = CreateController();
        var userId = Guid.NewGuid();
        var lessonId = Guid.NewGuid();

        _mockUserLessonProgressRepository.Setup(x => x.LearnedLessons(userId, lessonId))
            .ReturnsAsync(false);

        // Act
        var result = await controller.MarkLearnLesson(userId, lessonId);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().BeOfType<APIResponse>();
        var apiResponse = badRequestResult.Value as APIResponse;
        apiResponse!.errorMessages.Should().Contain("Server Error");
    }

    [Fact]
    public async Task MarkLearnLesson_WhenRepositoryThrowsException_ReturnsBadRequest() {
        // Arrange
        var controller = CreateController();
        var userId = Guid.NewGuid();
        var lessonId = Guid.NewGuid();

        _mockUserLessonProgressRepository.Setup(x => x.LearnedLessons(userId, lessonId))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await controller.MarkLearnLesson(userId, lessonId);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().BeOfType<APIResponse>();
        var apiResponse = badRequestResult.Value as APIResponse;
        apiResponse!.errorMessages.Should().Contain("Database error");
    }

    #endregion

    #region Edge Cases

    [Fact]
    public async Task CreateNewUserLesson_WithEmptyGuids_ReturnsOk() {
        // Arrange
        var controller = CreateController();
        var userId = Guid.Empty;
        var lessonId = Guid.Empty;

        _mockUserLessonProgressRepository.Setup(x => x.CreateNewLessonProgress(userId, lessonId))
            .ReturnsAsync(true);

        // Act
        var result = await controller.CreateNewUserLesson(userId, lessonId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task MarkCompleteLesson_WithEmptyGuids_ReturnsOk() {
        // Arrange
        var controller = CreateController();
        var userId = Guid.Empty;
        var lessonId = Guid.Empty;

        _mockUserLessonProgressRepository.Setup(x => x.CompleteLessons(userId, lessonId))
            .ReturnsAsync(true);

        // Act
        var result = await controller.MarkCompleteLesson(userId, lessonId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task MarkLearnLesson_WithEmptyGuids_ReturnsOk() {
        // Arrange
        var controller = CreateController();
        var userId = Guid.Empty;
        var lessonId = Guid.Empty;

        _mockUserLessonProgressRepository.Setup(x => x.LearnedLessons(userId, lessonId))
            .ReturnsAsync(true);

        // Act
        var result = await controller.MarkLearnLesson(userId, lessonId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    #endregion
}
