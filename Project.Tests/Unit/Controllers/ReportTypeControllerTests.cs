using Microsoft.AspNetCore.Mvc;
using Moq;
using Project.API.Controllers;
using Project.Core.Entities.Business.DTOs;
using Project.Core.Entities.General;
using Project.Core.Interfaces.IRepositories;
using Xunit;
using FluentAssertions;

namespace Project.Tests.Unit.Controllers;

public class ReportTypeControllerTests {
    private readonly Mock<IReportTypeRepository> _mockReportTypeRepository = new();

    private ReportTypeController CreateController() {
        return new ReportTypeController(_mockReportTypeRepository.Object);
    }

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_WithValidData_ReturnsOkWithReportTypes() {
        // Arrange
        var controller = CreateController();
        var reportTypes = new List<ReportType> {
            new() { Id = Guid.NewGuid(), Name = "Bug Report" },
            new() { Id = Guid.NewGuid(), Name = "Feature Request" },
            new() { Id = Guid.NewGuid(), Name = "General Feedback" }
        };

        _mockReportTypeRepository.Setup(x => x.GetAll()).ReturnsAsync(reportTypes);

        // Act
        var result = await controller.GetAllAsync();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeOfType<APIResponse>();
        var apiResponse = okResult.Value as APIResponse;
        apiResponse!.result.Should().BeAssignableTo<IEnumerable<ReportType>>();
        var returnedReportTypes = apiResponse.result as IEnumerable<ReportType>;
        returnedReportTypes!.Should().HaveCount(3);
        returnedReportTypes.ElementAt(0).Name.Should().Be("Bug Report");
        returnedReportTypes.ElementAt(1).Name.Should().Be("Feature Request");
        returnedReportTypes.ElementAt(2).Name.Should().Be("General Feedback");
    }

    [Fact]
    public async Task GetAllAsync_WithEmptyList_ReturnsOkWithEmptyList() {
        // Arrange
        var controller = CreateController();
        var reportTypes = new List<ReportType>();

        _mockReportTypeRepository.Setup(x => x.GetAll()).ReturnsAsync(reportTypes);

        // Act
        var result = await controller.GetAllAsync();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeOfType<APIResponse>();
        var apiResponse = okResult.Value as APIResponse;
        apiResponse!.result.Should().BeAssignableTo<IEnumerable<ReportType>>();
        var returnedReportTypes = apiResponse.result as IEnumerable<ReportType>;
        returnedReportTypes!.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_WithSingleReportType_ReturnsOkWithSingleReportType() {
        // Arrange
        var controller = CreateController();
        var reportTypes = new List<ReportType> {
            new() { Id = Guid.NewGuid(), Name = "Bug Report" }
        };

        _mockReportTypeRepository.Setup(x => x.GetAll()).ReturnsAsync(reportTypes);

        // Act
        var result = await controller.GetAllAsync();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeOfType<APIResponse>();
        var apiResponse = okResult.Value as APIResponse;
        apiResponse!.result.Should().BeAssignableTo<IEnumerable<ReportType>>();
        var returnedReportTypes = apiResponse.result as IEnumerable<ReportType>;
        returnedReportTypes!.Should().HaveCount(1);
        returnedReportTypes.ElementAt(0).Name.Should().Be("Bug Report");
    }

    [Fact]
    public async Task GetAllAsync_WithReportTypesWithEmptyName_ReturnsOkWithReportTypes() {
        // Arrange
        var controller = CreateController();
        var reportTypes = new List<ReportType> {
            new() { Id = Guid.NewGuid(), Name = "" }, // Empty name is allowed by entity
            new() { Id = Guid.NewGuid(), Name = "Valid Name" }
        };

        _mockReportTypeRepository.Setup(x => x.GetAll()).ReturnsAsync(reportTypes);

        // Act
        var result = await controller.GetAllAsync();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeOfType<APIResponse>();
        var apiResponse = okResult.Value as APIResponse;
        apiResponse!.result.Should().BeAssignableTo<IEnumerable<ReportType>>();
        var returnedReportTypes = apiResponse.result as IEnumerable<ReportType>;
        returnedReportTypes!.Should().HaveCount(2);
        returnedReportTypes.ElementAt(0).Name.Should().Be("");
        returnedReportTypes.ElementAt(1).Name.Should().Be("Valid Name");
    }

    [Fact]
    public async Task GetAllAsync_WithReportTypesWithWhitespaceName_ReturnsOkWithReportTypes() {
        // Arrange
        var controller = CreateController();
        var reportTypes = new List<ReportType> {
            new() { Id = Guid.NewGuid(), Name = "   " }, // Whitespace name is allowed by entity
            new() { Id = Guid.NewGuid(), Name = "Valid Name" }
        };

        _mockReportTypeRepository.Setup(x => x.GetAll()).ReturnsAsync(reportTypes);

        // Act
        var result = await controller.GetAllAsync();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeOfType<APIResponse>();
        var apiResponse = okResult.Value as APIResponse;
        apiResponse!.result.Should().BeAssignableTo<IEnumerable<ReportType>>();
        var returnedReportTypes = apiResponse.result as IEnumerable<ReportType>;
        returnedReportTypes!.Should().HaveCount(2);
        returnedReportTypes.ElementAt(0).Name.Should().Be("   ");
        returnedReportTypes.ElementAt(1).Name.Should().Be("Valid Name");
    }

    #endregion

    // Note: ReportTypeController has proper dependency injection via interfaces for all dependencies,
    // so all functionality can be properly unit tested. The controller does not have exception handling
    // (no try-catch blocks), so we don't test exception scenarios.
    // 
    // Note: The controller only has one method (GetAllAsync) that simply calls repository.GetAll()
    // and wraps the result in an APIResponse. There are no validation checks or complex logic to test.
} 