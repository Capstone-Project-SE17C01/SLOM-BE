using Project.API.Controllers;
using Project.Core.Entities.General;
using Project.Core.Interfaces.IRepositories;
using Xunit;

namespace Project.Tests.Unit.Controllers;

public class ModuleControllerTests {
    private readonly Mock<IModuleRepository> _mockModuleRepository = new();

    private ModuleController CreateController() {
        return new ModuleController(_mockModuleRepository.Object);
    }

    #region GetOngoingUserLesson Tests

    [Fact]
    public async Task GetOngoingUserLesson_WithValidCourseId_ReturnsOkWithModules() {
        // Arrange
        var controller = CreateController();
        var courseId = Guid.NewGuid();
        var expectedModules = new List<Module>
        {
            new Module { Id = Guid.NewGuid(), Title = "Module 1", CourseId = courseId, CreatedAt = DateTime.UtcNow },
            new Module { Id = Guid.NewGuid(), Title = "Module 2", CourseId = courseId, CreatedAt = DateTime.UtcNow }
        };

        _mockModuleRepository.Setup(x => x.GetModuleByCourseId(courseId))
            .ReturnsAsync(expectedModules);

        // Act
        var result = await controller.GetOngoingUserLesson(courseId);

        // Assert
        result.Should().NotBeNull();
        result.result.Should().BeOfType<List<Module>>();
        var modules = result.result as List<Module>;
        modules!.Should().HaveCount(2);
        modules[0].Title.Should().Be("Module 1");
        modules[1].Title.Should().Be("Module 2");
    }

    [Fact]
    public async Task GetOngoingUserLesson_WithNonExistentCourseId_ReturnsOkWithEmptyList() {
        // Arrange
        var controller = CreateController();
        var courseId = Guid.NewGuid();

        _mockModuleRepository.Setup(x => x.GetModuleByCourseId(courseId))
            .ReturnsAsync(new List<Module>());

        // Act
        var result = await controller.GetOngoingUserLesson(courseId);

        // Assert
        result.Should().NotBeNull();
        result.result.Should().BeOfType<List<Module>>();
        var modules = result.result as List<Module>;
        modules!.Should().BeEmpty();
    }

    [Fact]
    public async Task GetOngoingUserLesson_WithEmptyCourseId_ReturnsOkWithEmptyList() {
        // Arrange
        var controller = CreateController();
        var courseId = Guid.Empty;

        _mockModuleRepository.Setup(x => x.GetModuleByCourseId(courseId))
            .ReturnsAsync(new List<Module>());

        // Act
        var result = await controller.GetOngoingUserLesson(courseId);

        // Assert
        result.Should().NotBeNull();
        result.result.Should().BeOfType<List<Module>>();
        var modules = result.result as List<Module>;
        modules!.Should().BeEmpty();
    }

    [Fact]
    public async Task GetOngoingUserLesson_WithNullModules_ReturnsOkWithNull() {
        // Arrange
        var controller = CreateController();
        var courseId = Guid.NewGuid();

        _mockModuleRepository.Setup(x => x.GetModuleByCourseId(courseId))
            .ReturnsAsync((List<Module>?)null);

        // Act
        var result = await controller.GetOngoingUserLesson(courseId);

        // Assert
        result.Should().NotBeNull();
        result.result.Should().BeNull();
    }

    #endregion

    #region Edge Cases

    [Fact]
    public async Task GetOngoingUserLesson_WithLargeCourseId_ReturnsOk() {
        // Arrange
        var controller = CreateController();
        var courseId = Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF");
        var expectedModules = new List<Module>();

        _mockModuleRepository.Setup(x => x.GetModuleByCourseId(courseId))
            .ReturnsAsync(expectedModules);

        // Act
        var result = await controller.GetOngoingUserLesson(courseId);

        // Assert
        result.Should().NotBeNull();
        result.result.Should().BeOfType<List<Module>>();
        var modules = result.result as List<Module>;
        modules!.Should().BeEmpty();
    }

    #endregion

    // Note: ModuleController does not have exception handling, so we don't test exception scenarios
    // If exception handling is needed, it should be added to the controller first
}
