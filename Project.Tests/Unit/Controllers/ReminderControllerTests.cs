using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Project.API.Controllers;
using Project.Core.Entities.Business.DTOs;
using Project.Core.Entities.Business.DTOs.ReminderDTOs;
using Project.Core.Entities.General;
using Project.Core.Interfaces.IMapper;
using Project.Core.Interfaces.IRepositories;
using Xunit;

namespace Project.Tests.Unit.Controllers;

public class ReminderControllerTests {
    private readonly Mock<IReminderRepository> _mockReminderRepository = new();
    private readonly Mock<IBaseMapper<CreateReminderDTO, Reminder>> _mockMapper = new();

    private ReminderController CreateController() {
        return new ReminderController(_mockReminderRepository.Object, _mockMapper.Object);
    }

    #region GetReminder Tests

    [Fact]
    public async Task GetReminder_WithValidEmail_ReturnsOkWithReminder() {
        // Arrange
        var controller = CreateController();
        var email = "test@example.com";
        var reminder = new Reminder {
            Id = Guid.NewGuid(),
            Email = email,
            Message = "Test reminder message",
            TimeToSend = new TimeOnly(9, 0),
            IsActive = true,
            UserId = Guid.NewGuid()
        };

        _mockReminderRepository.Setup(x => x.GetReminderByEmailAsync(email, true)).ReturnsAsync(reminder);

        // Act
        var result = await controller.GetReminder(email);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeOfType<APIResponse>();
        var apiResponse = okResult.Value as APIResponse;
        apiResponse!.result.Should().BeOfType<Reminder>();
        var returnedReminder = apiResponse.result as Reminder;
        returnedReminder!.Email.Should().Be(email);
        returnedReminder.Message.Should().Be("Test reminder message");
    }

    [Fact]
    public async Task GetReminder_WithEmptyEmail_ReturnsBadRequest() {
        // Arrange
        var controller = CreateController();
        var email = "";

        // Act
        var result = await controller.GetReminder(email);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().BeOfType<APIResponse>();
        var apiResponse = badRequestResult.Value as APIResponse;
        apiResponse!.errorMessages.Should().Contain("Email is required");
    }

    [Fact]
    public async Task GetReminder_WithWhitespaceEmail_ReturnsBadRequest() {
        // Arrange
        var controller = CreateController();
        var email = "   ";

        // Act
        var result = await controller.GetReminder(email);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().BeOfType<APIResponse>();
        var apiResponse = badRequestResult.Value as APIResponse;
        apiResponse!.errorMessages.Should().Contain("Email is required");
    }

    [Fact]
    public async Task GetReminder_WithNullEmail_ReturnsBadRequest() {
        // Arrange
        var controller = CreateController();
        string? email = null;

        // Act
        var result = await controller.GetReminder(email);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().BeOfType<APIResponse>();
        var apiResponse = badRequestResult.Value as APIResponse;
        apiResponse!.errorMessages.Should().Contain("Email is required");
    }

    [Fact]
    public async Task GetReminder_WithNonExistentEmail_ReturnsNotFound() {
        // Arrange
        var controller = CreateController();
        var email = "nonexistent@example.com";

        _mockReminderRepository.Setup(x => x.GetReminderByEmailAsync(email, true)).ReturnsAsync((Reminder?)null);

        // Act
        var result = await controller.GetReminder(email);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult!.Value.Should().BeOfType<APIResponse>();
        var apiResponse = notFoundResult.Value as APIResponse;
        apiResponse!.errorMessages.Should().Contain("No active reminder found for this email");
    }

    #endregion

    #region SetupReminder Tests

    [Fact]
    public async Task SetupReminder_WithValidRequestAndExistingReminder_ReturnsOkWithUpdatedReminder() {
        // Arrange
        var controller = CreateController();
        var requestDTO = new CreateReminderDTO {
            Email = "test@example.com",
            Message = "Updated reminder message",
            TimeToSend = new TimeOnly(10, 30),
            IsActive = true,
            UserId = Guid.NewGuid()
        };
        var existingReminder = new Reminder {
            Id = Guid.NewGuid(),
            Email = requestDTO.Email,
            Message = "Old message",
            TimeToSend = new TimeOnly(9, 0),
            IsActive = false,
            UserId = requestDTO.UserId,
            LastSentDate = DateTime.UtcNow
        };

        _mockReminderRepository.Setup(x => x.GetReminderByEmailAsync(requestDTO.Email, false)).ReturnsAsync(existingReminder);
        _mockReminderRepository.Setup(x => x.Update(existingReminder)).Returns(Task.CompletedTask);

        // Act
        var result = await controller.SetupReminder(requestDTO);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeOfType<APIResponse>();
        var apiResponse = okResult.Value as APIResponse;
        apiResponse!.result.Should().BeOfType<Reminder>();
        var returnedReminder = apiResponse.result as Reminder;
        returnedReminder!.Message.Should().Be("Updated reminder message");
        returnedReminder.TimeToSend.Should().Be(new TimeOnly(10, 30));
        returnedReminder.IsActive.Should().BeTrue();
        returnedReminder.LastSentDate.Should().BeNull();
    }

    [Fact]
    public async Task SetupReminder_WithValidRequestAndNoExistingReminder_ReturnsOkWithCreatedReminder() {
        // Arrange
        var controller = CreateController();
        var requestDTO = new CreateReminderDTO {
            Email = "new@example.com",
            Message = "New reminder message",
            TimeToSend = new TimeOnly(14, 0),
            IsActive = true,
            UserId = Guid.NewGuid()
        };
        var mappedReminder = new Reminder {
            Email = requestDTO.Email,
            Message = requestDTO.Message,
            TimeToSend = requestDTO.TimeToSend,
            IsActive = requestDTO.IsActive,
            UserId = requestDTO.UserId
        };
        var createdReminder = new Reminder {
            Id = Guid.NewGuid(),
            Email = requestDTO.Email,
            Message = requestDTO.Message,
            TimeToSend = requestDTO.TimeToSend,
            IsActive = requestDTO.IsActive,
            UserId = requestDTO.UserId
        };

        _mockReminderRepository.Setup(x => x.GetReminderByEmailAsync(requestDTO.Email, false)).ReturnsAsync((Reminder?)null);
        _mockMapper.Setup(x => x.MapModel(requestDTO)).Returns(mappedReminder);
        _mockReminderRepository.Setup(x => x.Create(mappedReminder)).ReturnsAsync(createdReminder);

        // Act
        var result = await controller.SetupReminder(requestDTO);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeOfType<APIResponse>();
        var apiResponse = okResult.Value as APIResponse;
        apiResponse!.result.Should().BeOfType<Reminder>();
        var returnedReminder = apiResponse.result as Reminder;
        returnedReminder!.Email.Should().Be("new@example.com");
        returnedReminder.Message.Should().Be("New reminder message");
    }

    [Fact]
    public async Task SetupReminder_WithNullMessage_ReturnsOkWithCreatedReminder() {
        // Arrange
        var controller = CreateController();
        var requestDTO = new CreateReminderDTO {
            Email = "test@example.com",
            Message = null, // Null message is allowed
            TimeToSend = new TimeOnly(9, 0),
            IsActive = true,
            UserId = Guid.NewGuid()
        };
        var mappedReminder = new Reminder {
            Email = requestDTO.Email,
            Message = requestDTO.Message,
            TimeToSend = requestDTO.TimeToSend,
            IsActive = requestDTO.IsActive,
            UserId = requestDTO.UserId
        };
        var createdReminder = new Reminder {
            Id = Guid.NewGuid(),
            Email = requestDTO.Email,
            Message = requestDTO.Message,
            TimeToSend = requestDTO.TimeToSend,
            IsActive = requestDTO.IsActive,
            UserId = requestDTO.UserId
        };

        _mockReminderRepository.Setup(x => x.GetReminderByEmailAsync(requestDTO.Email, false)).ReturnsAsync((Reminder?)null);
        _mockMapper.Setup(x => x.MapModel(requestDTO)).Returns(mappedReminder);
        _mockReminderRepository.Setup(x => x.Create(mappedReminder)).ReturnsAsync(createdReminder);

        // Act
        var result = await controller.SetupReminder(requestDTO);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeOfType<APIResponse>();
        var apiResponse = okResult.Value as APIResponse;
        apiResponse!.result.Should().BeOfType<Reminder>();
        var returnedReminder = apiResponse.result as Reminder;
        returnedReminder!.Message.Should().BeNull();
    }

    [Fact]
    public async Task SetupReminder_WithNullUserId_ReturnsOkWithCreatedReminder() {
        // Arrange
        var controller = CreateController();
        var requestDTO = new CreateReminderDTO {
            Email = "test@example.com",
            Message = "Test message",
            TimeToSend = new TimeOnly(9, 0),
            IsActive = true,
            UserId = null // Null UserId is allowed
        };
        var mappedReminder = new Reminder {
            Email = requestDTO.Email,
            Message = requestDTO.Message,
            TimeToSend = requestDTO.TimeToSend,
            IsActive = requestDTO.IsActive,
            UserId = requestDTO.UserId
        };
        var createdReminder = new Reminder {
            Id = Guid.NewGuid(),
            Email = requestDTO.Email,
            Message = requestDTO.Message,
            TimeToSend = requestDTO.TimeToSend,
            IsActive = requestDTO.IsActive,
            UserId = requestDTO.UserId
        };

        _mockReminderRepository.Setup(x => x.GetReminderByEmailAsync(requestDTO.Email, false)).ReturnsAsync((Reminder?)null);
        _mockMapper.Setup(x => x.MapModel(requestDTO)).Returns(mappedReminder);
        _mockReminderRepository.Setup(x => x.Create(mappedReminder)).ReturnsAsync(createdReminder);

        // Act
        var result = await controller.SetupReminder(requestDTO);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeOfType<APIResponse>();
        var apiResponse = okResult.Value as APIResponse;
        apiResponse!.result.Should().BeOfType<Reminder>();
        var returnedReminder = apiResponse.result as Reminder;
        returnedReminder!.UserId.Should().BeNull();
    }

    [Fact]
    public async Task SetupReminder_WithFalseIsActive_ReturnsOkWithCreatedReminder() {
        // Arrange
        var controller = CreateController();
        var requestDTO = new CreateReminderDTO {
            Email = "test@example.com",
            Message = "Test message",
            TimeToSend = new TimeOnly(9, 0),
            IsActive = false, // False IsActive is allowed
            UserId = Guid.NewGuid()
        };
        var mappedReminder = new Reminder {
            Email = requestDTO.Email,
            Message = requestDTO.Message,
            TimeToSend = requestDTO.TimeToSend,
            IsActive = requestDTO.IsActive,
            UserId = requestDTO.UserId
        };
        var createdReminder = new Reminder {
            Id = Guid.NewGuid(),
            Email = requestDTO.Email,
            Message = requestDTO.Message,
            TimeToSend = requestDTO.TimeToSend,
            IsActive = requestDTO.IsActive,
            UserId = requestDTO.UserId
        };

        _mockReminderRepository.Setup(x => x.GetReminderByEmailAsync(requestDTO.Email, false)).ReturnsAsync((Reminder?)null);
        _mockMapper.Setup(x => x.MapModel(requestDTO)).Returns(mappedReminder);
        _mockReminderRepository.Setup(x => x.Create(mappedReminder)).ReturnsAsync(createdReminder);

        // Act
        var result = await controller.SetupReminder(requestDTO);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeOfType<APIResponse>();
        var apiResponse = okResult.Value as APIResponse;
        apiResponse!.result.Should().BeOfType<Reminder>();
        var returnedReminder = apiResponse.result as Reminder;
        returnedReminder!.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task SetupReminder_WithMapperReturnsNull_ReturnsBadRequest() {
        // Arrange
        var controller = CreateController();
        var requestDTO = new CreateReminderDTO {
            Email = "test@example.com",
            Message = "Test message",
            TimeToSend = new TimeOnly(9, 0),
            IsActive = true,
            UserId = Guid.NewGuid()
        };

        _mockReminderRepository.Setup(x => x.GetReminderByEmailAsync(requestDTO.Email, false)).ReturnsAsync((Reminder?)null);
        _mockMapper.Setup(x => x.MapModel(requestDTO)).Returns((Reminder?)null);

        // Act
        var result = await controller.SetupReminder(requestDTO);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().BeOfType<APIResponse>();
        var apiResponse = badRequestResult.Value as APIResponse;
        apiResponse!.errorMessages.Should().Contain("Mapping failed");
    }

    [Fact]
    public async Task SetupReminder_WithRepositoryException_ReturnsInternalServerError() {
        // Arrange
        var controller = CreateController();
        var requestDTO = new CreateReminderDTO {
            Email = "test@example.com",
            Message = "Test message",
            TimeToSend = new TimeOnly(9, 0),
            IsActive = true,
            UserId = Guid.NewGuid()
        };

        _mockReminderRepository.Setup(x => x.GetReminderByEmailAsync(requestDTO.Email, false))
            .ThrowsAsync(new Exception("Database connection failed"));

        // Act
        var result = await controller.SetupReminder(requestDTO);

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be(500);
        objectResult.Value.Should().BeOfType<APIResponse>();
        var apiResponse = objectResult.Value as APIResponse;
        apiResponse!.errorMessages.Should().Contain("Unexpected server error");
    }

    [Fact]
    public async Task SetupReminder_WithMapperException_ReturnsInternalServerError() {
        // Arrange
        var controller = CreateController();
        var requestDTO = new CreateReminderDTO {
            Email = "test@example.com",
            Message = "Test message",
            TimeToSend = new TimeOnly(9, 0),
            IsActive = true,
            UserId = Guid.NewGuid()
        };

        _mockReminderRepository.Setup(x => x.GetReminderByEmailAsync(requestDTO.Email, false)).ReturnsAsync((Reminder?)null);
        _mockMapper.Setup(x => x.MapModel(requestDTO))
            .Throws(new Exception("Mapping failed"));

        // Act
        var result = await controller.SetupReminder(requestDTO);

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be(500);
        objectResult.Value.Should().BeOfType<APIResponse>();
        var apiResponse = objectResult.Value as APIResponse;
        apiResponse!.errorMessages.Should().Contain("Unexpected server error");
    }

    #endregion

    // Note: ReminderController has proper dependency injection via interfaces for all dependencies,
    // so all functionality can be properly unit tested. The controller has exception handling
    // in the SetupReminder method, so we test exception scenarios for that method.
    // 
    // Note: The controller checks if reminder == null from mapper.MapModel() (which can return null),
    // but IBaseRepository.Create() method returns Task<T> (Task<Reminder>), not null, so we don't test
    // repository returning null scenarios as they don't reflect the actual interface contract.
}
