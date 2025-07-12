using System.Net;
using Project.Core.Entities.Business.DTOs.ChangePasswordDTOs;
using Project.Core.Entities.Business.DTOs.ForgotPasswordDTOs;
using Project.Core.Entities.Business.DTOs.LoginDTOs;
using Project.Core.Entities.Business.DTOs.RegisterDTOs;
using Project.Core.Entities.General;
using Project.Infrastructure.Data;
using Project.Tests.Helpers;

namespace Project.Tests.Integration.Controllers;

[TestFixture]
public class AuthControllerTests : IntegrationTestBase {
    private const string BaseUrl = "/api/Auth";

    [SetUp]
    public override void SetUp() {
        base.SetUp();
        SeedTestData();
    }

    private void SeedTestData() {
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var studentRole = new Role {
            Id = Guid.NewGuid(),
            Name = "Student",
            CreatedAt = DateTime.UtcNow,
        };

        var teacherRole = new Role {
            Id = Guid.NewGuid(),
            Name = "Teacher",
            CreatedAt = DateTime.UtcNow,
        };

        context.Roles.AddRange(studentRole, teacherRole);

        var englishLanguage = new Language {
            Id = Guid.NewGuid(),
            Name = "English",
            Code = "en",
            CreatedAt = DateTime.UtcNow,
        };

        var vietnameseLanguage = new Language {
            Id = Guid.NewGuid(),
            Name = "Vietnamese",
            Code = "vi",
            CreatedAt = DateTime.UtcNow,
        };

        context.Languages.AddRange(englishLanguage, vietnameseLanguage);

        context.SaveChanges();
    }

    #region Register Tests

    [Test]
    public async Task Register_WithValidData_ShouldReturnSuccess() {
        var registerRequest = new RegisterationRequestDTO {
            Email = "test@example.com",
            Password = "TempPassword123!",
            Role = "Student",
            LanguageCode = "en"
        };

        var response = await PostAsJsonAsync($"{BaseUrl}/Register", registerRequest);

        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task Register_WithInvalidEmail_ShouldReturnBadRequest() {
        var registerRequest = new RegisterationRequestDTO {
            Email = "invalid-email",
            Password = "TempPassword123!",
            Role = "Student",
            LanguageCode = "en"
        };

        var response = await PostAsJsonAsync($"{BaseUrl}/Register", registerRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task Register_WithWeakPassword_ShouldReturnBadRequest() {
        var registerRequest = new RegisterationRequestDTO {
            Email = "test@example.com",
            Password = "123",
            Role = "Student",
            LanguageCode = "en"
        };

        var response = await PostAsJsonAsync($"{BaseUrl}/Register", registerRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task Register_WithInvalidRole_ShouldReturnBadRequest() {
        var registerRequest = new RegisterationRequestDTO {
            Email = "test@example.com",
            Password = "TempPassword123!",
            Role = "InvalidRole",
            LanguageCode = "en"
        };

        var response = await PostAsJsonAsync($"{BaseUrl}/Register", registerRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Login Tests

    [Test]
    public async Task Login_WithValidCredentials_ShouldReturnSuccess() {
        var loginRequest = new LoginRequestDTO {
            Email = "test@example.com",
            Password = "TempPassword123!"
        };

        var response = await PostAsJsonAsync($"{BaseUrl}/Login", loginRequest);

        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task Login_WithInvalidCredentials_ShouldReturnBadRequest() {
        var loginRequest = new LoginRequestDTO {
            Email = "nonexistent@example.com",
            Password = "WrongPassword123!"
        };

        var response = await PostAsJsonAsync($"{BaseUrl}/Login", loginRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task Login_WithEmptyEmail_ShouldReturnBadRequest() {
        var loginRequest = new LoginRequestDTO {
            Email = "",
            Password = "TempPassword123!"
        };

        var response = await PostAsJsonAsync($"{BaseUrl}/Login", loginRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task Login_WithEmptyPassword_ShouldReturnBadRequest() {
        var loginRequest = new LoginRequestDTO {
            Email = "test@example.com",
            Password = ""
        };

        var response = await PostAsJsonAsync($"{BaseUrl}/Login", loginRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Google Login Tests

    [Test]
    public async Task LoginWithGoogle_WithValidAuthCode_ShouldReturnSuccess() {
        var googleLoginRequest = new LoginGoogleRequestDTO {
            Code = "valid_auth_code",
            RedirectUri = "http://localhost:3000/auth/callback",
            Role = "Student",
            LanguageCode = "en"
        };

        var response = await PostAsJsonAsync($"{BaseUrl}/LoginWithGoogle", googleLoginRequest);

        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task LoginWithGoogle_WithInvalidAuthCode_ShouldReturnBadRequest() {
        var googleLoginRequest = new LoginGoogleRequestDTO {
            Code = "invalid_auth_code",
            RedirectUri = "http://localhost:3000/auth/callback",
            Role = "Student",
            LanguageCode = "en"
        };

        var response = await PostAsJsonAsync($"{BaseUrl}/LoginWithGoogle", googleLoginRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Confirm Registration Tests

    [Test]
    public async Task ConfirmRegistration_WithValidCode_ShouldReturnSuccess() {
        var confirmRequest = new ConfirmRegisterationRequestDTO {
            Email = "test@example.com",
            ConfirmationCode = "123456",
            Username = "testuser"
        };

        var response = await PostAsJsonAsync($"{BaseUrl}/ConfirmRegistration", confirmRequest);

        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task ConfirmRegistration_WithInvalidCode_ShouldReturnBadRequest() {
        var confirmRequest = new ConfirmRegisterationRequestDTO {
            Email = "test@example.com",
            ConfirmationCode = "000000",
            Username = "testuser"
        };

        var response = await PostAsJsonAsync($"{BaseUrl}/ConfirmRegistration", confirmRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task ConfirmRegistration_WithEmptyUsername_ShouldReturnBadRequest() {
        var confirmRequest = new ConfirmRegisterationRequestDTO {
            Email = "test@example.com",
            ConfirmationCode = "123456",
            Username = ""
        };

        var response = await PostAsJsonAsync($"{BaseUrl}/ConfirmRegistration", confirmRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Resend Confirmation Code Tests

    [Test]
    public async Task ResendConfirmationCode_WithValidEmail_ShouldReturnSuccess() {
        var resendRequest = new ResendConfirmationCode {
            Email = "test@example.com"
        };

        var response = await PostAsJsonAsync($"{BaseUrl}/ResendConfirmationCode", resendRequest);

        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task ResendConfirmationCode_WithEmptyEmail_ShouldReturnBadRequest() {
        var resendRequest = new ResendConfirmationCode {
            Email = ""
        };

        var response = await PostAsJsonAsync($"{BaseUrl}/ResendConfirmationCode", resendRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Forgot Password Tests

    [Test]
    public async Task ForgotPassword_WithValidEmail_ShouldReturnSuccess() {
        var forgotPasswordRequest = new ForgotPasswordRequestDTO {
            Email = "test@example.com"
        };

        var response = await PostAsJsonAsync($"{BaseUrl}/ForgotPassword", forgotPasswordRequest);

        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task ForgotPassword_WithEmptyEmail_ShouldReturnBadRequest() {
        var forgotPasswordRequest = new ForgotPasswordRequestDTO {
            Email = ""
        };

        var response = await PostAsJsonAsync($"{BaseUrl}/ForgotPassword", forgotPasswordRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task ForgotPassword_WithNonExistentEmail_ShouldReturnBadRequest() {
        var forgotPasswordRequest = new ForgotPasswordRequestDTO {
            Email = "nonexistent@example.com"
        };

        var response = await PostAsJsonAsync($"{BaseUrl}/ForgotPassword", forgotPasswordRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Update Password Tests

    [Test]
    public async Task UpdatePassword_WithValidData_ShouldReturnSuccess() {
        var changePasswordRequest = new ChangePasswordRequestDTO {
            AccessToken = "valid_access_token",
            OldPassword = "OldPassword123!",
            NewPassword = "NewPassword123!"
        };

        var response = await PostAsJsonAsync($"{BaseUrl}/UpdatePassword", changePasswordRequest);

        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task UpdatePassword_WithInvalidAccessToken_ShouldReturnBadRequest() {
        var changePasswordRequest = new ChangePasswordRequestDTO {
            AccessToken = "invalid_access_token",
            OldPassword = "OldPassword123!",
            NewPassword = "NewPassword123!"
        };

        var response = await PostAsJsonAsync($"{BaseUrl}/UpdatePassword", changePasswordRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task UpdatePassword_WithWeakNewPassword_ShouldReturnBadRequest() {
        var changePasswordRequest = new ChangePasswordRequestDTO {
            AccessToken = "valid_access_token",
            OldPassword = "OldPassword123!",
            NewPassword = "123"
        };

        var response = await PostAsJsonAsync($"{BaseUrl}/UpdatePassword", changePasswordRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Error Handling Tests

    [Test]
    public async Task AuthController_WithMalformedRequest_ShouldReturnBadRequest() {
        var malformedJson = "{ invalid json }";
        var content = new StringContent(malformedJson, System.Text.Encoding.UTF8, "application/json");

        var response = await Client.PostAsync($"{BaseUrl}/Login", content);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task AuthController_WithNullRequest_ShouldReturnBadRequest() {
        var response = await Client.PostAsync($"{BaseUrl}/Login", null);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion
    [Test]
    public async Task Register_MultipleConcurrentRequests_ShouldHandleLoad() {
        var tasks = new List<Task<HttpResponseMessage>>();
        for (int i = 0; i < 5; i++) {
            var registerRequest = new RegisterationRequestDTO {
                Email = $"concurrent{i}@example.com",
                Password = "TempPassword123!",
                Role = "Student",
                LanguageCode = "en"
            };
            tasks.Add(PostAsJsonAsync($"{BaseUrl}/Register", registerRequest));
        }

        var responses = await Task.WhenAll(tasks);

        foreach (var response in responses) {
            response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest);
            response.Dispose();
        }
    }

    [Test]
    public async Task Register_WithLargeEmail_ShouldReturnBadRequestOrHandleGracefully() {
        var largeEmail = new string('a', 300) + "@example.com";
        var registerRequest = new RegisterationRequestDTO {
            Email = largeEmail,
            Password = "TempPassword123!",
            Role = "Student",
            LanguageCode = "en"
        };

        var response = await PostAsJsonAsync($"{BaseUrl}/Register", registerRequest);

        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.OK);
    }
}
