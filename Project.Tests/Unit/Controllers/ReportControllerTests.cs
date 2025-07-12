using Microsoft.AspNetCore.Mvc;
using Moq;
using Project.API.Controllers;
using Project.Core.Entities.Business.DTOs;
using Project.Core.Entities.Business.DTOs.ReportDTOs;
using Project.Core.Entities.General;
using Project.Core.Interfaces.IMapper;
using Project.Core.Interfaces.IRepositories;
using Xunit;
using FluentAssertions;

namespace Project.Tests.Unit.Controllers;

public class ReportControllerTests {
    private readonly Mock<IReportRepository> _mockReportRepository = new();
    private readonly Mock<IBaseMapper<CreateReportRequestDTO, Report>> _mockMapper = new();

    private ReportController CreateController() {
        return new ReportController(_mockReportRepository.Object, _mockMapper.Object);
    }

    #region CreateReport Tests

    [Fact]
    public async Task CreateReport_WithValidRequest_ReturnsOkWithSuccessMessage() {
        // Arrange
        var controller = CreateController();
        var createRequest = new CreateReportRequestDTO {
            Title = "Test Report",
            Content = "This is a test report content",
            ReportTypeId = Guid.NewGuid(),
            Status = true,
            UserId = Guid.NewGuid()
        };
        var mappedReport = new Report {
            Id = Guid.NewGuid(),
            Title = createRequest.Title,
            Content = createRequest.Content,
            ReportTypeId = createRequest.ReportTypeId,
            Status = createRequest.Status,
            UserId = createRequest.UserId
        };
        var createdReport = new Report {
            Id = mappedReport.Id,
            Title = mappedReport.Title,
            Content = mappedReport.Content,
            ReportTypeId = mappedReport.ReportTypeId,
            Status = mappedReport.Status,
            UserId = mappedReport.UserId
        };

        _mockMapper.Setup(x => x.MapModel(createRequest)).Returns(mappedReport);
        _mockReportRepository.Setup(x => x.Create(mappedReport)).ReturnsAsync(createdReport);

        // Act
        var result = await controller.CreateReport(createRequest);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeOfType<APIResponse>();
        var apiResponse = okResult.Value as APIResponse;
        apiResponse!.result.Should().Be("Created successfully");
    }

    [Fact]
    public async Task CreateReport_WithNullRequest_ReturnsBadRequest() {
        // Arrange
        var controller = CreateController();
        CreateReportRequestDTO? createRequest = null;

        // Act
        var result = await controller.CreateReport(createRequest);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().NotBeNull();
        
        // Use reflection to access the error property since it's an anonymous type
        var errorProperty = badRequestResult.Value.GetType().GetProperty("error");
        errorProperty.Should().NotBeNull();
        var errorValue = errorProperty!.GetValue(badRequestResult.Value);
        errorValue.Should().Be("Invalid report data.");
    }

    [Fact]
    public async Task CreateReport_WithEmptyTitle_ReturnsOkWithSuccessMessage() {
        // Arrange
        var controller = CreateController();
        var createRequest = new CreateReportRequestDTO {
            Title = "", // Empty title is allowed by the DTO
            Content = "This is a test report content",
            ReportTypeId = Guid.NewGuid(),
            Status = true,
            UserId = Guid.NewGuid()
        };
        var mappedReport = new Report {
            Id = Guid.NewGuid(),
            Title = createRequest.Title,
            Content = createRequest.Content,
            ReportTypeId = createRequest.ReportTypeId,
            Status = createRequest.Status,
            UserId = createRequest.UserId
        };
        var createdReport = new Report {
            Id = mappedReport.Id,
            Title = mappedReport.Title,
            Content = mappedReport.Content,
            ReportTypeId = mappedReport.ReportTypeId,
            Status = mappedReport.Status,
            UserId = mappedReport.UserId
        };

        _mockMapper.Setup(x => x.MapModel(createRequest)).Returns(mappedReport);
        _mockReportRepository.Setup(x => x.Create(mappedReport)).ReturnsAsync(createdReport);

        // Act
        var result = await controller.CreateReport(createRequest);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeOfType<APIResponse>();
        var apiResponse = okResult.Value as APIResponse;
        apiResponse!.result.Should().Be("Created successfully");
    }

    [Fact]
    public async Task CreateReport_WithNullContent_ReturnsOkWithSuccessMessage() {
        // Arrange
        var controller = CreateController();
        var createRequest = new CreateReportRequestDTO {
            Title = "Test Report",
            Content = null, // Null content is allowed by the DTO
            ReportTypeId = Guid.NewGuid(),
            Status = true,
            UserId = Guid.NewGuid()
        };
        var mappedReport = new Report {
            Id = Guid.NewGuid(),
            Title = createRequest.Title,
            Content = createRequest.Content,
            ReportTypeId = createRequest.ReportTypeId,
            Status = createRequest.Status,
            UserId = createRequest.UserId
        };
        var createdReport = new Report {
            Id = mappedReport.Id,
            Title = mappedReport.Title,
            Content = mappedReport.Content,
            ReportTypeId = mappedReport.ReportTypeId,
            Status = mappedReport.Status,
            UserId = mappedReport.UserId
        };

        _mockMapper.Setup(x => x.MapModel(createRequest)).Returns(mappedReport);
        _mockReportRepository.Setup(x => x.Create(mappedReport)).ReturnsAsync(createdReport);

        // Act
        var result = await controller.CreateReport(createRequest);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeOfType<APIResponse>();
        var apiResponse = okResult.Value as APIResponse;
        apiResponse!.result.Should().Be("Created successfully");
    }

    [Fact]
    public async Task CreateReport_WithRepositoryException_ReturnsInternalServerError() {
        // Arrange
        var controller = CreateController();
        var createRequest = new CreateReportRequestDTO {
            Title = "Test Report",
            Content = "This is a test report content",
            ReportTypeId = Guid.NewGuid(),
            Status = true,
            UserId = Guid.NewGuid()
        };
        var mappedReport = new Report {
            Id = Guid.NewGuid(),
            Title = createRequest.Title,
            Content = createRequest.Content,
            ReportTypeId = createRequest.ReportTypeId,
            Status = createRequest.Status,
            UserId = createRequest.UserId
        };

        _mockMapper.Setup(x => x.MapModel(createRequest)).Returns(mappedReport);
        _mockReportRepository.Setup(x => x.Create(mappedReport))
            .ThrowsAsync(new Exception("Database connection failed"));

        // Act
        var result = await controller.CreateReport(createRequest);

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be(500);
        objectResult.Value.Should().NotBeNull();
        
        // Use reflection to access the error property since it's an anonymous type
        var errorProperty = objectResult.Value.GetType().GetProperty("error");
        errorProperty.Should().NotBeNull();
        var errorValue = errorProperty!.GetValue(objectResult.Value);
        errorValue.Should().Be("Database connection failed");
    }

    [Fact]
    public async Task CreateReport_WithMapperException_ReturnsInternalServerError() {
        // Arrange
        var controller = CreateController();
        var createRequest = new CreateReportRequestDTO {
            Title = "Test Report",
            Content = "This is a test report content",
            ReportTypeId = Guid.NewGuid(),
            Status = true,
            UserId = Guid.NewGuid()
        };

        _mockMapper.Setup(x => x.MapModel(createRequest))
            .Throws(new Exception("Mapping failed"));

        // Act
        var result = await controller.CreateReport(createRequest);

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be(500);
        objectResult.Value.Should().NotBeNull();
        
        // Use reflection to access the error property since it's an anonymous type
        var errorProperty = objectResult.Value.GetType().GetProperty("error");
        errorProperty.Should().NotBeNull();
        var errorValue = errorProperty!.GetValue(objectResult.Value);
        errorValue.Should().Be("Mapping failed");
    }

    #endregion

    #region Edge Cases

    [Fact]
    public async Task CreateReport_WithWhitespaceTitle_ReturnsOkWithSuccessMessage() {
        // Arrange
        var controller = CreateController();
        var createRequest = new CreateReportRequestDTO {
            Title = "   ", // Whitespace title is allowed by the DTO
            Content = "This is a test report content",
            ReportTypeId = Guid.NewGuid(),
            Status = true,
            UserId = Guid.NewGuid()
        };
        var mappedReport = new Report {
            Id = Guid.NewGuid(),
            Title = createRequest.Title,
            Content = createRequest.Content,
            ReportTypeId = createRequest.ReportTypeId,
            Status = createRequest.Status,
            UserId = createRequest.UserId
        };
        var createdReport = new Report {
            Id = mappedReport.Id,
            Title = mappedReport.Title,
            Content = mappedReport.Content,
            ReportTypeId = mappedReport.ReportTypeId,
            Status = mappedReport.Status,
            UserId = mappedReport.UserId
        };

        _mockMapper.Setup(x => x.MapModel(createRequest)).Returns(mappedReport);
        _mockReportRepository.Setup(x => x.Create(mappedReport)).ReturnsAsync(createdReport);

        // Act
        var result = await controller.CreateReport(createRequest);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeOfType<APIResponse>();
        var apiResponse = okResult.Value as APIResponse;
        apiResponse!.result.Should().Be("Created successfully");
    }

    [Fact]
    public async Task CreateReport_WithFalseStatus_ReturnsOkWithSuccessMessage() {
        // Arrange
        var controller = CreateController();
        var createRequest = new CreateReportRequestDTO {
            Title = "Test Report",
            Content = "This is a test report content",
            ReportTypeId = Guid.NewGuid(),
            Status = false, // False status is allowed
            UserId = Guid.NewGuid()
        };
        var mappedReport = new Report {
            Id = Guid.NewGuid(),
            Title = createRequest.Title,
            Content = createRequest.Content,
            ReportTypeId = createRequest.ReportTypeId,
            Status = createRequest.Status,
            UserId = createRequest.UserId
        };
        var createdReport = new Report {
            Id = mappedReport.Id,
            Title = mappedReport.Title,
            Content = mappedReport.Content,
            ReportTypeId = mappedReport.ReportTypeId,
            Status = mappedReport.Status,
            UserId = mappedReport.UserId
        };

        _mockMapper.Setup(x => x.MapModel(createRequest)).Returns(mappedReport);
        _mockReportRepository.Setup(x => x.Create(mappedReport)).ReturnsAsync(createdReport);

        // Act
        var result = await controller.CreateReport(createRequest);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeOfType<APIResponse>();
        var apiResponse = okResult.Value as APIResponse;
        apiResponse!.result.Should().Be("Created successfully");
    }

    #endregion

    // Note: ReportController has proper dependency injection via interfaces for all dependencies,
    // so all functionality can be properly unit tested. The controller has exception handling
    // in the CreateReport method, so we test exception scenarios for that method.
    // 
    // Note: The controller checks if result == null from repository.Create(), but according to
    // IBaseRepository interface, Create method returns Task<T> (Task<Report>), not null.
    // This null check might be unnecessary, but we test the controller's actual behavior.
} 