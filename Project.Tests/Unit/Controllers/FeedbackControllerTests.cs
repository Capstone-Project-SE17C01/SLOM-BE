using Microsoft.AspNetCore.Mvc;
using Project.API.Controllers;
using Project.Core.Entities.General;
using Project.Core.Interfaces.IRepositories;
using Xunit;

namespace Project.Tests.Unit.Controllers;

public class FeedbackControllerTests {
    private readonly Mock<IFeedbackRepository> _mockRepository = new();

    private FeedbackController CreateController() {
        return new FeedbackController(_mockRepository.Object);
    }

    #region GetAll Tests

    [Fact]
    public async Task GetAll_WithValidData_ReturnsOkWithFeedbacks() {
        // Arrange
        var controller = CreateController();
        var expectedFeedbacks = new List<Feedback>
        {
            new Feedback { Id = 1, Name = "John Doe", Email = "john@example.com", Subject = "Feedback 1", Message = "Content 1" },
            new Feedback { Id = 2, Name = "Jane Smith", Email = "jane@example.com", Subject = "Feedback 2", Message = "Content 2" }
        };

        _mockRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(expectedFeedbacks);

        // Act
        var result = await controller.GetAll();

        // Assert
        result.Should().BeOfType<ActionResult<IEnumerable<Feedback>>>();
        var actionResult = result as ActionResult<IEnumerable<Feedback>>;
        actionResult!.Result.Should().BeOfType<OkObjectResult>();
        var okResult = actionResult.Result as OkObjectResult;
        okResult!.Value.Should().BeAssignableTo<IEnumerable<Feedback>>();
        var feedbacks = okResult.Value as IEnumerable<Feedback>;
        feedbacks!.Should().HaveCount(2);
        feedbacks.ElementAt(0).Subject.Should().Be("Feedback 1");
        feedbacks.ElementAt(1).Subject.Should().Be("Feedback 2");
    }

    [Fact]
    public async Task GetAll_WithEmptyList_ReturnsOkWithEmptyList() {
        // Arrange
        var controller = CreateController();
        var emptyFeedbacks = new List<Feedback>();

        _mockRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(emptyFeedbacks);

        // Act
        var result = await controller.GetAll();

        // Assert
        result.Should().BeOfType<ActionResult<IEnumerable<Feedback>>>();
        var actionResult = result as ActionResult<IEnumerable<Feedback>>;
        actionResult!.Result.Should().BeOfType<OkObjectResult>();
        var okResult = actionResult.Result as OkObjectResult;
        okResult!.Value.Should().BeAssignableTo<IEnumerable<Feedback>>();
        var feedbacks = okResult.Value as IEnumerable<Feedback>;
        feedbacks!.Should().BeEmpty();
    }

    #endregion

    #region Get Tests

    [Fact]
    public async Task Get_WithValidId_ReturnsOkWithFeedback() {
        // Arrange
        var controller = CreateController();
        var feedbackId = 1;
        var expectedFeedback = new Feedback { Id = feedbackId, Name = "John Doe", Email = "john@example.com", Subject = "Feedback 1", Message = "Content 1" };

        _mockRepository.Setup(x => x.GetByIdAsync(feedbackId)).ReturnsAsync(expectedFeedback);

        // Act
        var result = await controller.Get(feedbackId);

        // Assert
        result.Should().BeOfType<ActionResult<Feedback>>();
        var actionResult = result as ActionResult<Feedback>;
        actionResult!.Result.Should().BeOfType<OkObjectResult>();
        var okResult = actionResult.Result as OkObjectResult;
        okResult!.Value.Should().BeOfType<Feedback>();
        var feedback = okResult.Value as Feedback;
        feedback!.Id.Should().Be(feedbackId);
        feedback.Subject.Should().Be("Feedback 1");
    }

    [Fact]
    public async Task Get_WithNonExistentId_ReturnsNotFound() {
        // Arrange
        var controller = CreateController();
        var feedbackId = 999;

        _mockRepository.Setup(x => x.GetByIdAsync(feedbackId)).ReturnsAsync((Feedback?)null);

        // Act
        var result = await controller.Get(feedbackId);

        // Assert
        result.Should().BeOfType<ActionResult<Feedback>>();
        var actionResult = result as ActionResult<Feedback>;
        actionResult!.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Get_WithZeroId_ReturnsNotFound() {
        // Arrange
        var controller = CreateController();
        var feedbackId = 0;

        _mockRepository.Setup(x => x.GetByIdAsync(feedbackId)).ReturnsAsync((Feedback?)null);

        // Act
        var result = await controller.Get(feedbackId);

        // Assert
        result.Should().BeOfType<ActionResult<Feedback>>();
        var actionResult = result as ActionResult<Feedback>;
        actionResult!.Result.Should().BeOfType<NotFoundResult>();
    }

    #endregion

    #region Create Tests

    [Fact]
    public async Task Create_WithValidFeedback_ReturnsCreatedAtAction() {
        // Arrange
        var controller = CreateController();
        var feedback = new Feedback { Name = "New User", Email = "new@example.com", Subject = "New Feedback", Message = "New Content" };
        var createdFeedback = new Feedback { Id = 1, Name = "New User", Email = "new@example.com", Subject = "New Feedback", Message = "New Content" };

        _mockRepository.Setup(x => x.AddAsync(feedback)).ReturnsAsync(createdFeedback);

        // Act
        var result = await controller.Create(feedback);

        // Assert
        result.Should().BeOfType<ActionResult<Feedback>>();
        var actionResult = result as ActionResult<Feedback>;
        actionResult!.Result.Should().BeOfType<CreatedAtActionResult>();
        var createdAtResult = actionResult.Result as CreatedAtActionResult;
        createdAtResult!.ActionName.Should().Be("Get");
        createdAtResult.RouteValues!["id"].Should().Be(1);
        createdAtResult.Value.Should().BeOfType<Feedback>();
        var created = createdAtResult.Value as Feedback;
        created!.Id.Should().Be(1);
        created.Subject.Should().Be("New Feedback");
    }

    [Fact]
    public async Task Create_WithNullFeedback_ReturnsCreatedAtAction() {
        // Arrange
        var controller = CreateController();
        Feedback? feedback = null;
        var createdFeedback = new Feedback { Id = 1, Name = "", Email = "", Subject = "", Message = "" };

        _mockRepository.Setup(x => x.AddAsync(feedback)).ReturnsAsync(createdFeedback);

        // Act
        var result = await controller.Create(feedback);

        // Assert
        result.Should().BeOfType<ActionResult<Feedback>>();
        var actionResult = result as ActionResult<Feedback>;
        actionResult!.Result.Should().BeOfType<CreatedAtActionResult>();
    }

    #endregion

    #region Update Tests

    [Fact]
    public async Task Update_WithValidData_ReturnsNoContent() {
        // Arrange
        var controller = CreateController();
        var feedbackId = 1;
        var feedback = new Feedback { Id = feedbackId, Name = "Updated User", Email = "updated@example.com", Subject = "Updated Feedback", Message = "Updated Content" };

        _mockRepository.Setup(x => x.UpdateAsync(feedback)).Returns(Task.CompletedTask);

        // Act
        var result = await controller.Update(feedbackId, feedback);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Update_WithMismatchedId_ReturnsBadRequest() {
        // Arrange
        var controller = CreateController();
        var feedbackId = 1;
        var feedback = new Feedback { Id = 2, Name = "Updated User", Email = "updated@example.com", Subject = "Updated Feedback", Message = "Updated Content" };

        // Act
        var result = await controller.Update(feedbackId, feedback);

        // Assert
        result.Should().BeOfType<BadRequestResult>();
    }

    [Fact]
    public async Task Update_WithNullFeedback_ThrowsNullReferenceException() {
        // Arrange
        var controller = CreateController();
        var feedbackId = 1;
        Feedback? feedback = null;

        // Act & Assert
        var action = () => controller.Update(feedbackId, feedback);
        await action.Should().ThrowAsync<NullReferenceException>();
    }

    #endregion

    #region Delete Tests

    [Fact]
    public async Task Delete_WithValidId_ReturnsNoContent() {
        // Arrange
        var controller = CreateController();
        var feedbackId = 1;

        _mockRepository.Setup(x => x.DeleteAsync(feedbackId)).Returns(Task.CompletedTask);

        // Act
        var result = await controller.Delete(feedbackId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Delete_WithZeroId_ReturnsNoContent() {
        // Arrange
        var controller = CreateController();
        var feedbackId = 0;

        _mockRepository.Setup(x => x.DeleteAsync(feedbackId)).Returns(Task.CompletedTask);

        // Act
        var result = await controller.Delete(feedbackId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    #endregion

    #region Edge Cases

    [Fact]
    public async Task Get_WithNegativeId_ReturnsNotFound() {
        // Arrange
        var controller = CreateController();
        var feedbackId = -1;

        _mockRepository.Setup(x => x.GetByIdAsync(feedbackId)).ReturnsAsync((Feedback?)null);

        // Act
        var result = await controller.Get(feedbackId);

        // Assert
        result.Should().BeOfType<ActionResult<Feedback>>();
        var actionResult = result as ActionResult<Feedback>;
        actionResult!.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Update_WithNegativeId_ReturnsNoContent() {
        // Arrange
        var controller = CreateController();
        var feedbackId = -1;
        var feedback = new Feedback { Id = -1, Name = "Updated User", Email = "updated@example.com", Subject = "Updated Feedback", Message = "Updated Content" };

        _mockRepository.Setup(x => x.UpdateAsync(feedback)).Returns(Task.CompletedTask);

        // Act
        var result = await controller.Update(feedbackId, feedback);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Delete_WithNegativeId_ReturnsNoContent() {
        // Arrange
        var controller = CreateController();
        var feedbackId = -1;

        _mockRepository.Setup(x => x.DeleteAsync(feedbackId)).Returns(Task.CompletedTask);

        // Act
        var result = await controller.Delete(feedbackId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    #endregion

    // Note: FeedbackController does not have exception handling (no try-catch blocks),
    // so we don't test exception scenarios. All dependencies are injected via interfaces,
    // so all functionality can be properly unit tested.
}
