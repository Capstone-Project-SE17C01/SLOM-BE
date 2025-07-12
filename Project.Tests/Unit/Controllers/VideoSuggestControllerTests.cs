using Microsoft.AspNetCore.Mvc;
using Project.API.Controllers;
using Project.Core.Entities.Business.DTOs;
using Project.Core.Entities.Business.DTOs.VideoSuggestDTOs;
using Project.Core.Entities.General;
using Project.Core.Interfaces.IRepositories;
using Xunit;

namespace Project.Tests.Unit.Controllers;

public class VideoSuggestControllerTests {
    private readonly Mock<IVideoSuggestRepository> _mockVideoRepo = new();
    private readonly Mock<IUserLessonProgressRepository> _mockUserLessonProgressRepo = new();

    private VideoSuggestController CreateController() {
        return new VideoSuggestController(_mockVideoRepo.Object, _mockUserLessonProgressRepo.Object);
    }

    #region GetVideoSuggest Tests

    [Fact]
    public async Task GetVideoSuggest_WithValidRequest_ReturnsOkWithVideoSuggests() {
        // Arrange
        var controller = CreateController();
        var requestDTO = new VideoSuggestRequestDTO {
            UserId = Guid.NewGuid(),
            PageNumber = 1,
            PageSize = 10
        };
        var expectedVideoSuggests = new VideoSuggestResponseDto {
            VideoSuggest = new List<ListVideoSuggest>
            {
                new ListVideoSuggest { Id = Guid.NewGuid(), Title = "Video 1", VideoUrl = "https://example.com/video1", VideoThumbnail = "https://example.com/thumb1.jpg", VideoId = "video1" },
                new ListVideoSuggest { Id = Guid.NewGuid(), Title = "Video 2", VideoUrl = "https://example.com/video2", VideoThumbnail = "https://example.com/thumb2.jpg", VideoId = "video2" }
            },
            isLoadFullPage = true
        };

        _mockVideoRepo.Setup(x => x.GetVideoSuggestsByUserId(requestDTO))
            .ReturnsAsync(expectedVideoSuggests);

        // Act
        var result = await controller.GetVideoSuggest(requestDTO);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeOfType<APIResponse>();
        var apiResponse = okResult.Value as APIResponse;
        apiResponse!.result.Should().BeOfType<VideoSuggestResponseDto>();
        var videoSuggestResponse = apiResponse.result as VideoSuggestResponseDto;
        videoSuggestResponse!.VideoSuggest.Should().HaveCount(2);
        videoSuggestResponse.VideoSuggest![0].Title.Should().Be("Video 1");
        videoSuggestResponse.VideoSuggest[1].Title.Should().Be("Video 2");
        videoSuggestResponse.isLoadFullPage.Should().BeTrue();
    }

    [Fact]
    public async Task GetVideoSuggest_WhenRepositoryReturnsNull_ReturnsNotFound() {
        // Arrange
        var controller = CreateController();
        var requestDTO = new VideoSuggestRequestDTO {
            UserId = Guid.NewGuid(),
            PageNumber = 1,
            PageSize = 10
        };

        _mockVideoRepo.Setup(x => x.GetVideoSuggestsByUserId(requestDTO))
            .ReturnsAsync((VideoSuggestResponseDto?)null);

        // Act
        var result = await controller.GetVideoSuggest(requestDTO);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult!.Value.Should().BeOfType<APIResponse>();
        var apiResponse = notFoundResult.Value as APIResponse;
        apiResponse!.errorMessages.Should().Contain("NoVideoSuggest");
    }

    [Fact]
    public async Task GetVideoSuggest_WhenRepositoryThrowsException_ReturnsInternalServerError() {
        // Arrange
        var controller = CreateController();
        var requestDTO = new VideoSuggestRequestDTO {
            UserId = Guid.NewGuid(),
            PageNumber = 1,
            PageSize = 10
        };

        _mockVideoRepo.Setup(x => x.GetVideoSuggestsByUserId(requestDTO))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await controller.GetVideoSuggest(requestDTO);

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be(500);
        objectResult.Value.Should().BeOfType<APIResponse>();
        var apiResponse = objectResult.Value as APIResponse;
        apiResponse!.errorMessages.Should().Contain("Error Video Suggest");
    }

    [Fact]
    public async Task GetVideoSuggest_WithEmptyVideoSuggests_ReturnsOkWithEmptyList() {
        // Arrange
        var controller = CreateController();
        var requestDTO = new VideoSuggestRequestDTO {
            UserId = Guid.NewGuid(),
            PageNumber = 1,
            PageSize = 10
        };
        var emptyVideoSuggests = new VideoSuggestResponseDto {
            VideoSuggest = new List<ListVideoSuggest>(),
            isLoadFullPage = false
        };

        _mockVideoRepo.Setup(x => x.GetVideoSuggestsByUserId(requestDTO))
            .ReturnsAsync(emptyVideoSuggests);

        // Act
        var result = await controller.GetVideoSuggest(requestDTO);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeOfType<APIResponse>();
        var apiResponse = okResult.Value as APIResponse;
        apiResponse!.result.Should().BeOfType<VideoSuggestResponseDto>();
        var videoSuggestResponse = apiResponse.result as VideoSuggestResponseDto;
        videoSuggestResponse!.VideoSuggest.Should().BeEmpty();
        videoSuggestResponse.isLoadFullPage.Should().BeFalse();
    }

    #endregion

    #region Edge Cases

    [Fact]
    public async Task GetVideoSuggest_WithNullRequest_ReturnsOk() {
        // Arrange
        var controller = CreateController();
        VideoSuggestRequestDTO? requestDTO = null;
        var expectedVideoSuggests = new VideoSuggestResponseDto {
            VideoSuggest = new List<ListVideoSuggest>(),
            isLoadFullPage = false
        };

        _mockVideoRepo.Setup(x => x.GetVideoSuggestsByUserId(requestDTO))
            .ReturnsAsync(expectedVideoSuggests);

        // Act
        var result = await controller.GetVideoSuggest(requestDTO);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetVideoSuggest_WithEmptyUserId_ReturnsOk() {
        // Arrange
        var controller = CreateController();
        var requestDTO = new VideoSuggestRequestDTO {
            UserId = Guid.Empty,
            PageNumber = 1,
            PageSize = 10
        };
        var expectedVideoSuggests = new VideoSuggestResponseDto {
            VideoSuggest = new List<ListVideoSuggest>(),
            isLoadFullPage = false
        };

        _mockVideoRepo.Setup(x => x.GetVideoSuggestsByUserId(requestDTO))
            .ReturnsAsync(expectedVideoSuggests);

        // Act
        var result = await controller.GetVideoSuggest(requestDTO);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetVideoSuggest_WithLargePageSize_ReturnsOk() {
        // Arrange
        var controller = CreateController();
        var requestDTO = new VideoSuggestRequestDTO {
            UserId = Guid.NewGuid(),
            PageNumber = 1,
            PageSize = 1000
        };
        var expectedVideoSuggests = new VideoSuggestResponseDto {
            VideoSuggest = new List<ListVideoSuggest>(),
            isLoadFullPage = false
        };

        _mockVideoRepo.Setup(x => x.GetVideoSuggestsByUserId(requestDTO))
            .ReturnsAsync(expectedVideoSuggests);

        // Act
        var result = await controller.GetVideoSuggest(requestDTO);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    #endregion
}
