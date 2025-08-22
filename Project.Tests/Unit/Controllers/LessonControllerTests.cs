using Project.API.Controllers;
using Project.Core.Entities.General;
using Project.Core.Interfaces.IRepositories;
using Xunit;

namespace Project.Tests.Unit.Controllers;

public class LessonControllerTests {
    private readonly Mock<IUserLessonProgressRepository> _mockUserLessonProgressRepository = new();
    private readonly Mock<IWordRepository> _mockWordRepository = new();
    private readonly Mock<ILessonRepository> _mockLessonRepository = new();

    private LessonController CreateController() {
        return new LessonController(
            _mockUserLessonProgressRepository.Object,
            _mockWordRepository.Object,
            _mockLessonRepository.Object
        );
    }

    #region GetOngoingUserLesson Tests

    [Fact]
    public async Task GetOngoingUserLesson_WithValidUserId_ReturnsOkWithLesson() {
        // Arrange
        var controller = CreateController();
        var userId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var expectedLesson = new Lesson {
            Id = Guid.NewGuid(),
            Title = "Active Lesson",
            Content = "Lesson content",
            CreatedAt = DateTime.UtcNow
        };

        _mockUserLessonProgressRepository.Setup(x => x.GetActiveLessonByUserIdAsync(userId, courseId))
            .ReturnsAsync(expectedLesson);

        // Act
        var result = await controller.GetOngoingUserLesson(userId, courseId);

        // Assert
        result.Should().NotBeNull();
        result.errorMessages.Should().BeNull();
        result.result.Should().BeOfType<Lesson>();
        var lesson = result.result as Lesson;
        lesson!.Id.Should().Be(expectedLesson.Id);
        lesson.Title.Should().Be(expectedLesson.Title);
    }

    [Fact]
    public async Task GetOngoingUserLesson_WithNonExistentUserId_ReturnsOkWithNull() {
        // Arrange
        var controller = CreateController();
        var userId = Guid.NewGuid();
        var courseId = Guid.NewGuid();

        _mockUserLessonProgressRepository.Setup(x => x.GetActiveLessonByUserIdAsync(userId, courseId))
            .ReturnsAsync((Lesson?)null);

        // Act
        var result = await controller.GetOngoingUserLesson(userId, courseId);

        // Assert
        result.Should().NotBeNull();
        result.errorMessages.Should().BeNull();
        result.result.Should().BeNull();
    }

    [Fact]
    public async Task GetOngoingUserLesson_WithEmptyUserId_ReturnsOkWithNull() {
        // Arrange
        var controller = CreateController();
        var userId = Guid.Empty;
        var courseId = Guid.NewGuid();

        _mockUserLessonProgressRepository.Setup(x => x.GetActiveLessonByUserIdAsync(userId, courseId))
            .ReturnsAsync((Lesson?)null);

        // Act
        var result = await controller.GetOngoingUserLesson(userId, courseId);

        // Assert
        result.Should().NotBeNull();
        result.errorMessages.Should().BeNull();
        result.result.Should().BeNull();
    }

    #endregion

    #region GetLearnedLesson Tests

    [Fact]
    public async Task GetLearnedLesson_WithValidUserId_ReturnsOkWithLessons() {
        // Arrange
        var controller = CreateController();
        var userId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var expectedLessons = new List<Lesson>
        {
            new Lesson { Id = Guid.NewGuid(), Title = "Lesson 1", CreatedAt = DateTime.UtcNow },
            new Lesson { Id = Guid.NewGuid(), Title = "Lesson 2", CreatedAt = DateTime.UtcNow }
        };

        _mockUserLessonProgressRepository.Setup(x => x.GetLearnedLessons(userId, courseId))
            .ReturnsAsync(expectedLessons);

        // Act
        var result = await controller.GetLearnedLesson(userId, courseId);

        // Assert
        result.Should().NotBeNull();
        result.errorMessages.Should().BeNull();
        result.result.Should().BeOfType<List<Lesson>>();
        var lessons = result.result as List<Lesson>;
        lessons!.Should().HaveCount(2);
        lessons[0].Title.Should().Be("Lesson 1");
        lessons[1].Title.Should().Be("Lesson 2");
    }

    [Fact]
    public async Task GetLearnedLesson_WithNonExistentUserId_ReturnsOkWithEmptyList() {
        // Arrange
        var controller = CreateController();
        var userId = Guid.NewGuid();
        var courseId = Guid.NewGuid();

        _mockUserLessonProgressRepository.Setup(x => x.GetLearnedLessons(userId, courseId))
            .ReturnsAsync(new List<Lesson>());

        // Act
        var result = await controller.GetLearnedLesson(userId, courseId);

        // Assert
        result.Should().NotBeNull();
        result.errorMessages.Should().BeNull();
        result.result.Should().BeOfType<List<Lesson>>();
        var lessons = result.result as List<Lesson>;
        lessons!.Should().BeEmpty();
    }

    [Fact]
    public async Task GetLearnedLesson_WithEmptyUserId_ReturnsOkWithEmptyList() {
        // Arrange
        var controller = CreateController();
        var userId = Guid.Empty;
        var courseId = Guid.NewGuid();

        _mockUserLessonProgressRepository.Setup(x => x.GetLearnedLessons(userId, courseId))
            .ReturnsAsync(new List<Lesson>());

        // Act
        var result = await controller.GetLearnedLesson(userId, courseId);

        // Assert
        result.Should().NotBeNull();
        result.errorMessages.Should().BeNull();
        result.result.Should().BeOfType<List<Lesson>>();
        var lessons = result.result as List<Lesson>;
        lessons!.Should().BeEmpty();
    }

    #endregion

    #region GetListWordLesson Tests

    [Fact]
    public async Task GetListWordLesson_WithValidLessonId_ReturnsOkWithWords() {
        // Arrange
        var controller = CreateController();
        var lessonId = Guid.NewGuid();
        var expectedWords = new List<Word>
        {
            new Word { Id = Guid.NewGuid(), Text = "Hello", LessonId = Guid.NewGuid() },
            new Word { Id = Guid.NewGuid(), Text = "Goodbye", LessonId = Guid.NewGuid() }
        };

        _mockWordRepository.Setup(x => x.GetWordByLessonId(lessonId))
            .ReturnsAsync(expectedWords);

        // Act
        var result = await controller.GetListWordLesson(lessonId);

        // Assert
        result.Should().NotBeNull();
        result.errorMessages.Should().BeNull();
        result.result.Should().BeOfType<List<Word>>();
        var words = result.result as List<Word>;
        words!.Should().HaveCount(2);
        words[0].Text.Should().Be("Hello");
        words[1].Text.Should().Be("Goodbye");
    }

    [Fact]
    public async Task GetListWordLesson_WithNonExistentLessonId_ReturnsOkWithEmptyList() {
        // Arrange
        var controller = CreateController();
        var lessonId = Guid.NewGuid();

        _mockWordRepository.Setup(x => x.GetWordByLessonId(lessonId))
            .ReturnsAsync(new List<Word>());

        // Act
        var result = await controller.GetListWordLesson(lessonId);

        // Assert
        result.Should().NotBeNull();
        result.errorMessages.Should().BeNull();
        result.result.Should().BeOfType<List<Word>>();
        var words = result.result as List<Word>;
        words!.Should().BeEmpty();
    }

    [Fact]
    public async Task GetListWordLesson_WithEmptyLessonId_ReturnsOkWithEmptyList() {
        // Arrange
        var controller = CreateController();
        var lessonId = Guid.Empty;

        _mockWordRepository.Setup(x => x.GetWordByLessonId(lessonId))
            .ReturnsAsync(new List<Word>());

        // Act
        var result = await controller.GetListWordLesson(lessonId);

        // Assert
        result.Should().NotBeNull();
        result.errorMessages.Should().BeNull();
        result.result.Should().BeOfType<List<Word>>();
        var words = result.result as List<Word>;
        words!.Should().BeEmpty();
    }

    #endregion


    // Note: LessonController does not have exception handling, so we don't test exception scenarios
    // If exception handling is needed, it should be added to the controller first
}
