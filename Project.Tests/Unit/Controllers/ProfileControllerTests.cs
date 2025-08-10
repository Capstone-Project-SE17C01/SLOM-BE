using Microsoft.AspNetCore.Mvc;
using Project.API.Controllers;
using Project.Core.Entities.Business.DTOs;
using Project.Core.Entities.Business.DTOs.LanguageDTOs;
using Project.Core.Entities.Business.DTOs.ProfileDTOs;
using Project.Core.Entities.General;
using Project.Core.Interfaces.IMapper;
using Project.Core.Interfaces.IRepositories;
using Xunit;

namespace Project.Tests.Unit.Controllers;

public class ProfileControllerTests {
    private readonly Mock<IProfileRepository> _mockProfileRepository = new();
    private readonly Mock<ILanguageRepository> _mockLanguageRepository = new();
    private readonly Mock<IBaseMapper<Profile, ProfileByEmailResponse>> _mockMapper = new();

    private ProfileController CreateController() {
        return new ProfileController(_mockProfileRepository.Object, _mockMapper.Object, _mockLanguageRepository.Object);
    }

    #region GetProfileById Tests

    [Fact]
    public async Task GetProfileById_WithValidEmail_ReturnsOkWithProfile() {
        // Arrange
        var controller = CreateController();
        var email = "test@example.com";
        var profile = new Profile {
            Id = Guid.NewGuid(),
            Email = email,
            Username = "TestUser",
            AvatarUrl = "avatar.jpg",
            PreferredLanguageId = Guid.NewGuid()
        };
        var expectedResponse = new ProfileByEmailResponse {
            Id = profile.Id,
            Email = profile.Email,
            Username = profile.Username,
            AvatarUrl = profile.AvatarUrl,
            PreferredLanguageId = profile.PreferredLanguageId
        };

        _mockProfileRepository.Setup(x => x.GetProfileByEmail(email)).ReturnsAsync(profile);
        _mockMapper.Setup(x => x.MapModel(profile)).Returns(expectedResponse);

        // Act
        var result = await controller.GetProfileById(email);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeOfType<APIResponse>();
        var apiResponse = okResult.Value as APIResponse;
        apiResponse!.result.Should().BeOfType<ProfileByEmailResponse>();
        var profileResponse = apiResponse.result as ProfileByEmailResponse;
        profileResponse!.Email.Should().Be(email);
        profileResponse.Username.Should().Be("TestUser");
    }

    [Fact]
    public async Task GetProfileById_WithNonExistentEmail_ReturnsNotFound() {
        // Arrange
        var controller = CreateController();
        var email = "nonexistent@example.com";

        _mockProfileRepository.Setup(x => x.GetProfileByEmail(email)).ReturnsAsync((Profile?)null);

        // Act
        var result = await controller.GetProfileById(email);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult!.Value.Should().BeOfType<APIResponse>();
        var apiResponse = notFoundResult.Value as APIResponse;
        apiResponse!.errorMessages.Should().Contain("Profile not found");
    }

    [Fact]
    public async Task GetProfileById_WithEmptyEmail_ReturnsNotFound() {
        // Arrange
        var controller = CreateController();
        var email = "";

        _mockProfileRepository.Setup(x => x.GetProfileByEmail(email)).ReturnsAsync((Profile?)null);

        // Act
        var result = await controller.GetProfileById(email);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult!.Value.Should().BeOfType<APIResponse>();
        var apiResponse = notFoundResult.Value as APIResponse;
        apiResponse!.errorMessages.Should().Contain("Profile not found");
    }

    #endregion

    #region GetProfilesByName Tests

    [Fact]
    public async Task GetProfilesByName_WithValidInput_ReturnsProfilesList() {
        // Arrange
        var controller = CreateController();
        var input = "John";
        var currentUserEmail = "current@example.com";
        var profiles = new List<ProfileByNameResponse> {
            new() { UserEmail = "john1@example.com", UserName = "John Doe", UserAvatar = "avatar1.jpg" },
            new() { UserEmail = "john2@example.com", UserName = "John Smith", UserAvatar = "avatar2.jpg" }
        };

        _mockProfileRepository.Setup(x => x.GetProfileByName(input, currentUserEmail)).ReturnsAsync(profiles);

        // Act
        var result = await controller.GetProfilesByName(input, currentUserEmail);

        // Assert
        result.Should().BeOfType<List<ProfileByNameResponse>>();
        result.Should().HaveCount(2);
        result[0].UserEmail.Should().Be("john1@example.com");
        result[1].UserEmail.Should().Be("john2@example.com");
    }

    [Fact]
    public async Task GetProfilesByName_WithEmptyInput_ReturnsEmptyList() {
        // Arrange
        var controller = CreateController();
        var input = "";
        var currentUserEmail = "current@example.com";
        var profiles = new List<ProfileByNameResponse>();

        _mockProfileRepository.Setup(x => x.GetProfileByName(input, currentUserEmail)).ReturnsAsync(profiles);

        // Act
        var result = await controller.GetProfilesByName(input, currentUserEmail);

        // Assert
        result.Should().BeOfType<List<ProfileByNameResponse>>();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetProfilesByName_WithNullProfiles_ReturnsEmptyList() {
        // Arrange
        var controller = CreateController();
        var input = "John";
        var currentUserEmail = "current@example.com";
        var profiles = new List<ProfileByNameResponse> { null!, null! };

        _mockProfileRepository.Setup(x => x.GetProfileByName(input, currentUserEmail)).ReturnsAsync(profiles);

        // Act
        var result = await controller.GetProfilesByName(input, currentUserEmail);

        // Assert
        result.Should().BeOfType<List<ProfileByNameResponse>>();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetProfilesByName_WithMixedNullAndValidProfiles_ReturnsOnlyValidProfiles() {
        // Arrange
        var controller = CreateController();
        var input = "John";
        var currentUserEmail = "current@example.com";
        var profiles = new List<ProfileByNameResponse> {
            null!,
            new() { UserEmail = "john@example.com", UserName = "John Doe", UserAvatar = "avatar.jpg" },
            null!
        };

        _mockProfileRepository.Setup(x => x.GetProfileByName(input, currentUserEmail)).ReturnsAsync(profiles);

        // Act
        var result = await controller.GetProfilesByName(input, currentUserEmail);

        // Assert
        result.Should().BeOfType<List<ProfileByNameResponse>>();
        result.Should().HaveCount(1);
        result[0].UserEmail.Should().Be("john@example.com");
    }

    #endregion

    #region ChangeLanguage Tests

    [Fact]
    public async Task ChangeLanguage_WithValidRequest_ReturnsOkWithLanguageResponse() {
        // Arrange
        var controller = CreateController();
        var request = new ChangeLanguageRequestDTO {
            Email = "test@example.com",
            LanguageId = Guid.NewGuid(),
            NewLanguageCode = "en"
        };
        var profile = new Profile {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PreferredLanguageId = Guid.NewGuid()
        };
        var language = new Language {
            Id = request.LanguageId,
            Code = request.NewLanguageCode
        };

        _mockProfileRepository.Setup(x => x.GetProfileByEmail(request.Email)).ReturnsAsync(profile);
        _mockLanguageRepository.Setup(x => x.GetLanguageByCodeAsync(request.NewLanguageCode)).ReturnsAsync(language);
        _mockProfileRepository.Setup(x => x.Update(profile)).Returns(Task.CompletedTask);

        // Act
        var result = await controller.ChangeLanguage(request);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeOfType<APIResponse>();
        var apiResponse = okResult.Value as APIResponse;
        apiResponse!.result.Should().BeOfType<ChangeLanguageResponseDTO>();
        var languageResponse = apiResponse.result as ChangeLanguageResponseDTO;
        languageResponse!.LanguageId.Should().Be(request.LanguageId);
        languageResponse.LanguageCode.Should().Be(request.NewLanguageCode);
    }

    [Fact]
    public async Task ChangeLanguage_WithNonExistentProfile_ReturnsNotFound() {
        // Arrange
        var controller = CreateController();
        var request = new ChangeLanguageRequestDTO {
            Email = "nonexistent@example.com",
            LanguageId = Guid.NewGuid(),
            NewLanguageCode = "en"
        };

        _mockProfileRepository.Setup(x => x.GetProfileByEmail(request.Email)).ReturnsAsync((Profile?)null);

        // Act
        var result = await controller.ChangeLanguage(request);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult!.Value.Should().BeOfType<APIResponse>();
        var apiResponse = notFoundResult.Value as APIResponse;
        apiResponse!.errorMessages.Should().Contain("Profile not found");
    }

    [Fact]
    public async Task ChangeLanguage_WithLanguageRepositoryException_ReturnsNotFound() {
        // Arrange
        var controller = CreateController();
        var request = new ChangeLanguageRequestDTO {
            Email = "test@example.com",
            LanguageId = Guid.NewGuid(),
            NewLanguageCode = "invalid"
        };
        var profile = new Profile {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PreferredLanguageId = Guid.NewGuid()
        };

        _mockProfileRepository.Setup(x => x.GetProfileByEmail(request.Email)).ReturnsAsync(profile);
        _mockLanguageRepository.Setup(x => x.GetLanguageByCodeAsync(request.NewLanguageCode))
            .ThrowsAsync(new Exception("Language not found"));

        // Act
        var result = await controller.ChangeLanguage(request);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult!.Value.Should().BeOfType<APIResponse>();
        var apiResponse = notFoundResult.Value as APIResponse;
        apiResponse!.errorMessages.Should().Contain("ChangeLanguageFailed");
    }

    [Fact]
    public async Task ChangeLanguage_WithProfileRepositoryException_ReturnsNotFound() {
        // Arrange
        var controller = CreateController();
        var request = new ChangeLanguageRequestDTO {
            Email = "test@example.com",
            LanguageId = Guid.NewGuid(),
            NewLanguageCode = "en"
        };
        var profile = new Profile {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PreferredLanguageId = Guid.NewGuid()
        };
        var language = new Language {
            Id = request.LanguageId,
            Code = request.NewLanguageCode
        };

        _mockProfileRepository.Setup(x => x.GetProfileByEmail(request.Email)).ReturnsAsync(profile);
        _mockLanguageRepository.Setup(x => x.GetLanguageByCodeAsync(request.NewLanguageCode)).ReturnsAsync(language);
        _mockProfileRepository.Setup(x => x.Update(profile)).ThrowsAsync(new Exception("Update failed"));

        // Act
        var result = await controller.ChangeLanguage(request);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult!.Value.Should().BeOfType<APIResponse>();
        var apiResponse = notFoundResult.Value as APIResponse;
        apiResponse!.errorMessages.Should().Contain("ChangeLanguageFailed");
    }

    #endregion

    #region Edge Cases

    [Fact]
    public async Task GetProfileById_WithWhitespaceEmail_ReturnsNotFound() {
        // Arrange
        var controller = CreateController();
        var email = "   ";

        _mockProfileRepository.Setup(x => x.GetProfileByEmail(email)).ReturnsAsync((Profile?)null);

        // Act
        var result = await controller.GetProfileById(email);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult!.Value.Should().BeOfType<APIResponse>();
        var apiResponse = notFoundResult.Value as APIResponse;
        apiResponse!.errorMessages.Should().Contain("Profile not found");
    }

    [Fact]
    public async Task GetProfilesByName_WithWhitespaceInput_ReturnsEmptyList() {
        // Arrange
        var controller = CreateController();
        var input = "   ";
        var currentUserEmail = "current@example.com";
        var profiles = new List<ProfileByNameResponse>();

        _mockProfileRepository.Setup(x => x.GetProfileByName(input, currentUserEmail)).ReturnsAsync(profiles);

        // Act
        var result = await controller.GetProfilesByName(input, currentUserEmail);

        // Assert
        result.Should().BeOfType<List<ProfileByNameResponse>>();
        result.Should().BeEmpty();
    }

    #endregion

    // Note: ProfileController has proper dependency injection via interfaces for all dependencies,
    // so all functionality can be properly unit tested. The controller also has exception handling
    // in the ChangeLanguage method, so we test exception scenarios for that method.
}
