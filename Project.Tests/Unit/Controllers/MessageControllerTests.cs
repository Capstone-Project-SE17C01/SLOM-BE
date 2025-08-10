using Project.API.Controllers;
using Project.Core.Entities.Business.DTOs.MessageDTOs;
using Project.Core.Interfaces.IRepositories;
using Xunit;

namespace Project.Tests.Unit.Controllers;

public class MessageControllerTests {
    private readonly Mock<IMessageRepository> _mockMessageRepository = new();

    private MessageController CreateController() {
        return new MessageController(_mockMessageRepository.Object);
    }

    #region GetUserMessage Tests

    [Fact]
    public async Task GetUserMessage_WithValidUserId_ReturnsUserMessages() {
        // Arrange
        var controller = CreateController();
        var userId = Guid.NewGuid();
        var expectedMessages = new List<MessageUserResponse> {
            new() {
                UserName = "John Doe",
                UserEmail = "john@example.com",
                LastMessage = "Hello there!",
                IsSender = true,
                IsSeen = true,
                Avatar = "avatar1.jpg",
                LastSent = "2024-01-15T10:30:00Z"
            },
            new() {
                UserName = "Jane Smith",
                UserEmail = "jane@example.com",
                LastMessage = "How are you?",
                IsSender = false,
                IsSeen = false,
                Avatar = "avatar2.jpg",
                LastSent = "2024-01-15T09:15:00Z"
            }
        };

        _mockMessageRepository.Setup(x => x.GetMessageUser(userId)).ReturnsAsync(expectedMessages);

        // Act
        var result = await controller.GetUserMessage(userId);

        // Assert
        result.Should().BeOfType<List<MessageUserResponse>>();
        result.Should().HaveCount(2);
        result[0].UserName.Should().Be("John Doe");
        result[0].UserEmail.Should().Be("john@example.com");
        result[0].IsSender.Should().BeTrue();
        result[0].IsSeen.Should().BeTrue();
        result[1].UserName.Should().Be("Jane Smith");
        result[1].UserEmail.Should().Be("jane@example.com");
        result[1].IsSender.Should().BeFalse();
        result[1].IsSeen.Should().BeFalse();
    }

    [Fact]
    public async Task GetUserMessage_WithEmptyList_ReturnsEmptyList() {
        // Arrange
        var controller = CreateController();
        var userId = Guid.NewGuid();
        var expectedMessages = new List<MessageUserResponse>();

        _mockMessageRepository.Setup(x => x.GetMessageUser(userId)).ReturnsAsync(expectedMessages);

        // Act
        var result = await controller.GetUserMessage(userId);

        // Assert
        result.Should().BeOfType<List<MessageUserResponse>>();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetUserMessage_WithSingleMessage_ReturnsSingleMessage() {
        // Arrange
        var controller = CreateController();
        var userId = Guid.NewGuid();
        var expectedMessages = new List<MessageUserResponse> {
            new() {
                UserName = "John Doe",
                UserEmail = "john@example.com",
                LastMessage = "Hello there!",
                IsSender = true,
                IsSeen = true,
                Avatar = "avatar1.jpg",
                LastSent = "2024-01-15T10:30:00Z"
            }
        };

        _mockMessageRepository.Setup(x => x.GetMessageUser(userId)).ReturnsAsync(expectedMessages);

        // Act
        var result = await controller.GetUserMessage(userId);

        // Assert
        result.Should().BeOfType<List<MessageUserResponse>>();
        result.Should().HaveCount(1);
        result[0].UserName.Should().Be("John Doe");
        result[0].IsSender.Should().BeTrue();
    }

    #endregion

    #region GetMessage Tests

    [Fact]
    public async Task GetMessage_WithValidParameters_ReturnsMessageResponse() {
        // Arrange
        var controller = CreateController();
        var userId = Guid.NewGuid();
        var receiverEmail = "receiver@example.com";
        var pageNumber = 1;
        var expectedResponse = new MessageResponse {
            IsLoadFullPage = true,
            Data = new List<MessageContent> {
                new() { Id = 1, Content = "Hello!", IsSender = true },
                new() { Id = 2, Content = "Hi there!", IsSender = false },
                new() { Id = 3, Content = "How are you?", IsSender = true }
            }
        };

        _mockMessageRepository.Setup(x => x.GetMessage(It.IsAny<MessageRequest>())).ReturnsAsync(expectedResponse);

        // Act
        var result = await controller.GetMessage(userId.ToString(), receiverEmail, pageNumber);

        // Assert
        result.Should().BeOfType<MessageResponse>();
        result.IsLoadFullPage.Should().BeTrue();
        result.Data.Should().HaveCount(3);
        result.Data[0].Content.Should().Be("Hello!");
        result.Data[0].IsSender.Should().BeTrue();
        result.Data[1].Content.Should().Be("Hi there!");
        result.Data[1].IsSender.Should().BeFalse();
        result.Data[2].Content.Should().Be("How are you?");
        result.Data[2].IsSender.Should().BeTrue();
    }

    [Fact]
    public async Task GetMessage_WithEmptyData_ReturnsEmptyMessageResponse() {
        // Arrange
        var controller = CreateController();
        var userId = Guid.NewGuid();
        var receiverEmail = "receiver@example.com";
        var pageNumber = 1;
        var expectedResponse = new MessageResponse {
            IsLoadFullPage = false,
            Data = new List<MessageContent>()
        };

        _mockMessageRepository.Setup(x => x.GetMessage(It.IsAny<MessageRequest>())).ReturnsAsync(expectedResponse);

        // Act
        var result = await controller.GetMessage(userId.ToString(), receiverEmail, pageNumber);

        // Assert
        result.Should().BeOfType<MessageResponse>();
        result.IsLoadFullPage.Should().BeFalse();
        result.Data.Should().BeEmpty();
    }

    [Fact]
    public async Task GetMessage_WithSingleMessage_ReturnsSingleMessageResponse() {
        // Arrange
        var controller = CreateController();
        var userId = Guid.NewGuid();
        var receiverEmail = "receiver@example.com";
        var pageNumber = 1;
        var expectedResponse = new MessageResponse {
            IsLoadFullPage = false,
            Data = new List<MessageContent> {
                new() { Id = 1, Content = "Hello!", IsSender = true }
            }
        };

        _mockMessageRepository.Setup(x => x.GetMessage(It.IsAny<MessageRequest>())).ReturnsAsync(expectedResponse);

        // Act
        var result = await controller.GetMessage(userId.ToString(), receiverEmail, pageNumber);

        // Assert
        result.Should().BeOfType<MessageResponse>();
        result.IsLoadFullPage.Should().BeFalse();
        result.Data.Should().HaveCount(1);
        result.Data[0].Content.Should().Be("Hello!");
        result.Data[0].IsSender.Should().BeTrue();
    }

    [Fact]
    public async Task GetMessage_WithZeroPageNumber_ReturnsMessageResponse() {
        // Arrange
        var controller = CreateController();
        var userId = Guid.NewGuid();
        var receiverEmail = "receiver@example.com";
        var pageNumber = 0; // Zero page number is allowed
        var expectedResponse = new MessageResponse {
            IsLoadFullPage = false,
            Data = new List<MessageContent> {
                new() { Id = 1, Content = "Hello!", IsSender = true }
            }
        };

        _mockMessageRepository.Setup(x => x.GetMessage(It.IsAny<MessageRequest>())).ReturnsAsync(expectedResponse);

        // Act
        var result = await controller.GetMessage(userId.ToString(), receiverEmail, pageNumber);

        // Assert
        result.Should().BeOfType<MessageResponse>();
        result.IsLoadFullPage.Should().BeFalse();
        result.Data.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetMessage_WithNegativePageNumber_ReturnsMessageResponse() {
        // Arrange
        var controller = CreateController();
        var userId = Guid.NewGuid();
        var receiverEmail = "receiver@example.com";
        var pageNumber = -1; // Negative page number is allowed
        var expectedResponse = new MessageResponse {
            IsLoadFullPage = false,
            Data = new List<MessageContent> {
                new() { Id = 1, Content = "Hello!", IsSender = true }
            }
        };

        _mockMessageRepository.Setup(x => x.GetMessage(It.IsAny<MessageRequest>())).ReturnsAsync(expectedResponse);

        // Act
        var result = await controller.GetMessage(userId.ToString(), receiverEmail, pageNumber);

        // Assert
        result.Should().BeOfType<MessageResponse>();
        result.IsLoadFullPage.Should().BeFalse();
        result.Data.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetMessage_WithEmptyReceiverEmail_ReturnsMessageResponse() {
        // Arrange
        var controller = CreateController();
        var userId = Guid.NewGuid();
        var receiverEmail = ""; // Empty email is allowed
        var pageNumber = 1;
        var expectedResponse = new MessageResponse {
            IsLoadFullPage = false,
            Data = new List<MessageContent>()
        };

        _mockMessageRepository.Setup(x => x.GetMessage(It.IsAny<MessageRequest>())).ReturnsAsync(expectedResponse);

        // Act
        var result = await controller.GetMessage(userId.ToString(), receiverEmail, pageNumber);

        // Assert
        result.Should().BeOfType<MessageResponse>();
        result.IsLoadFullPage.Should().BeFalse();
        result.Data.Should().BeEmpty();
    }

    [Fact]
    public async Task GetMessage_WithWhitespaceReceiverEmail_ReturnsMessageResponse() {
        // Arrange
        var controller = CreateController();
        var userId = Guid.NewGuid();
        var receiverEmail = "   "; // Whitespace email is allowed
        var pageNumber = 1;
        var expectedResponse = new MessageResponse {
            IsLoadFullPage = false,
            Data = new List<MessageContent>()
        };

        _mockMessageRepository.Setup(x => x.GetMessage(It.IsAny<MessageRequest>())).ReturnsAsync(expectedResponse);

        // Act
        var result = await controller.GetMessage(userId.ToString(), receiverEmail, pageNumber);

        // Assert
        result.Should().BeOfType<MessageResponse>();
        result.IsLoadFullPage.Should().BeFalse();
        result.Data.Should().BeEmpty();
    }

    #endregion

    #region Edge Cases

    [Fact]
    public async Task GetUserMessage_WithEmptyGuid_ReturnsUserMessages() {
        // Arrange
        var controller = CreateController();
        var userId = Guid.Empty; // Empty GUID is allowed
        var expectedMessages = new List<MessageUserResponse> {
            new() {
                UserName = "John Doe",
                UserEmail = "john@example.com",
                LastMessage = "Hello there!",
                IsSender = true,
                IsSeen = true,
                Avatar = "avatar1.jpg",
                LastSent = "2024-01-15T10:30:00Z"
            }
        };

        _mockMessageRepository.Setup(x => x.GetMessageUser(userId)).ReturnsAsync(expectedMessages);

        // Act
        var result = await controller.GetUserMessage(userId);

        // Assert
        result.Should().BeOfType<List<MessageUserResponse>>();
        result.Should().HaveCount(1);
        result[0].UserName.Should().Be("John Doe");
    }

    [Fact]
    public async Task GetMessage_WithLargePageNumber_ReturnsMessageResponse() {
        // Arrange
        var controller = CreateController();
        var userId = Guid.NewGuid();
        var receiverEmail = "receiver@example.com";
        var pageNumber = int.MaxValue; // Large page number is allowed
        var expectedResponse = new MessageResponse {
            IsLoadFullPage = false,
            Data = new List<MessageContent>()
        };

        _mockMessageRepository.Setup(x => x.GetMessage(It.IsAny<MessageRequest>())).ReturnsAsync(expectedResponse);

        // Act
        var result = await controller.GetMessage(userId.ToString(), receiverEmail, pageNumber);

        // Assert
        result.Should().BeOfType<MessageResponse>();
        result.IsLoadFullPage.Should().BeFalse();
        result.Data.Should().BeEmpty();
    }

    #endregion

    // Note: MessageController has proper dependency injection via interfaces for all dependencies,
    // so all functionality can be properly unit tested. The controller does not have exception handling
    // (no try-catch blocks), so we don't test exception scenarios.
    // 
    // Note: The controller methods are simple pass-through methods that call repository methods
    // and return the results directly. There are no validation checks or complex logic to test.
    // The GetMessage method parses the userId string to Guid, but this is handled by Guid.Parse()
    // which will throw FormatException for invalid GUIDs, but since the controller doesn't handle
    // exceptions, we don't test invalid GUID scenarios.
}
