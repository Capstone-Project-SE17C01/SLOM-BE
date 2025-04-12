using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Extensions.CognitoAuthentication;
using Microsoft.AspNetCore.Mvc;
using Project.Core.Entities.Business.DTOs;
using Project.Core.Entities.Business.DTOs.ForgotPasswordDTOs;
using Project.Core.Entities.Business.DTOs.LoginDTOs;
using Project.Core.Entities.Business.DTOs.RegisterDTOs;
using Project.Core.Interfaces.IRepositories;
using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;

namespace Project.API.Controllers
{
    [Route("api/Auth")]
    [ApiController]
    public class AuthController : ControllerBase {
        private readonly AmazonCognitoIdentityProviderClient _provider;
        private readonly CognitoUserPool _userPool;
        private readonly IConfiguration _configuration;
        private readonly IProfileRepository _profileRepository;
        private readonly ILanguageRepository _languageRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ApplicationDbContext _db;


        public AuthController(IConfiguration configuration, IServiceProvider serviceProvider, ApplicationDbContext db) {
            _configuration = configuration;
            _db = db;
            _profileRepository = new ProfileRepository(_db);
            _languageRepository = new LanguageRepository(_db);
            _roleRepository = new RoleRepository(_db);

            var accessKey = _configuration["AWS:AccessKey"];
            var secretKey = _configuration["AWS:SecretKey"];
            var region = _configuration["AWS:Region"];
            var userPoolId = _configuration["AWS:UserPoolId"];
            var clientId = _configuration["AWS:ClientId"];

            _provider = new AmazonCognitoIdentityProviderClient(
                accessKey,
                secretKey,
                RegionEndpoint.GetBySystemName(region));

            _userPool = new CognitoUserPool(userPoolId, clientId, _provider);
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterationRequestDTO registerDTO) {
            try {

                try {
                    var getUserRequest = new AdminGetUserRequest {
                        Username = registerDTO.email,
                        UserPoolId = _configuration["AWS:UserPoolId"]
                    };

                    var getUserResponse = await _provider.AdminGetUserAsync(getUserRequest);

                    if (getUserResponse.UserStatus == UserStatusType.UNCONFIRMED) {
                        var deleteUserRequest = new AdminDeleteUserRequest {
                            Username = registerDTO.email,
                            UserPoolId = _configuration["AWS:UserPoolId"]
                        };

                        await _provider.AdminDeleteUserAsync(deleteUserRequest);
                    } else {
                        return BadRequest();
                    }
                } catch (UserNotFoundException) {
                }


                #region Create User after checking if user exists
                var signUpRequest = new SignUpRequest {
                    ClientId = _configuration["AWS:ClientId"],
                    Username = registerDTO.email,
                    Password = registerDTO.password,
                    UserAttributes = new List<AttributeType>
                    {
                        new AttributeType
                        {
                            Name = "email",
                            Value = registerDTO.email
                        }
                    }
                };

                var response = await _provider.SignUpAsync(signUpRequest);

                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK) {

                    var langCode = registerDTO.languageCode != null ? registerDTO.languageCode.ToLower() : "en";
                    var enLangId = await _languageRepository.GetIdByCodeAsync(langCode);
                    var RoleId = await _roleRepository.GetIdByNameAsync(registerDTO.role);

                    var profile = new Core.Entities.General.Profile {
                        Username = registerDTO.email.Split('@')[0].Trim(),
                        Email = registerDTO.email.Trim(),
                        RoleId = RoleId,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        PreferredLanguageId = enLangId,
                        AvatarUrl = "https://avatar.iran.liara.run/public/5",
                        Id = Guid.Parse(response.UserSub),
                    };
                    try {
                        var registerProfile = await _profileRepository.Create(profile);
                        return Ok(new APIResponse() { result = registerProfile });
                    } catch (Exception ex) {
                        var deleteUserRequest = new AdminDeleteUserRequest {
                            Username = registerDTO.email,
                            UserPoolId = _configuration["AWS:UserPoolId"]
                        };

                        await _provider.AdminDeleteUserAsync(deleteUserRequest);
                        return BadRequest(new APIResponse() { errorMessages = new List<string> { ex.Message } });
                    }

                } else {
                    return BadRequest(new APIResponse() { errorMessages = new List<string> { "Error Profile create" } });
                }
                #endregion
            } catch (Exception ex) {
                List<string> errorMess = new List<string>();
                switch (ex) {
                    case InvalidPasswordException:
                        errorMess.Add(ex.Message.Split(":")[1]);
                        break;
                    case TooManyFailedAttemptsException:
                        errorMess.Add("Too many failed attempts");
                        break;
                    case UserNotFoundException:
                        errorMess.Add("User not found");
                        break;
                    case UsernameExistsException:
                        errorMess.Add("Username already exists");
                        break;
                    case NotAuthorizedException:
                        errorMess.Add("Not authorized");
                        break;

                    default:
                        errorMess.Add("An unknown error occurred");
                        break;
                }
                return BadRequest(new APIResponse() { errorMessages = errorMess });
            }

        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequestDTO loginRequestDTO) {
            var user = new CognitoUser(loginRequestDTO.email, _configuration["AWS:ClientId"], _userPool, _provider);

            var authRequest = new InitiateSrpAuthRequest {
                Password = loginRequestDTO.password
            };

            try {
                var authResponse = await user.StartWithSrpAuthAsync(authRequest).ConfigureAwait(false);
                if (authResponse.AuthenticationResult != null) {
                    var loginResponse = new LoginResponseDTO {
                        idToken = authResponse.AuthenticationResult.IdToken,
                        accessToken = authResponse.AuthenticationResult.AccessToken,
                        refreshToken = authResponse.AuthenticationResult.RefreshToken,
                        userEmail = loginRequestDTO.email
                    };
                    return Ok(new APIResponse() { result = loginResponse });
                } else {
                    return BadRequest(new APIResponse() { errorMessages = new List<string> { "Server error" } });
                }
            } catch (Exception ex) {
                return BadRequest(new APIResponse() { errorMessages = new List<string> { ex.Message } });
            }
        }

        [HttpPost("ConfirmRegistration")]
        public async Task<IActionResult> ConfirmRegistration(ConfirmRegisterationRequestDTO confirmRegisterationRequest) {
            try {
                if (confirmRegisterationRequest.isPasswordReset && !string.IsNullOrEmpty(confirmRegisterationRequest.newPassword)) {
                    return await HandlePasswordReset(confirmRegisterationRequest);
                } else {
                    return await HandleRegistrationConfirmation(confirmRegisterationRequest);
                }
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        private async Task<IActionResult> HandlePasswordReset(ConfirmRegisterationRequestDTO confirmRegisterationRequest) {
            if (string.IsNullOrEmpty(confirmRegisterationRequest.newPassword)) {
                return BadRequest();
            }

            var confirmForgotPasswordRequest = new ConfirmForgotPasswordRequest {
                ClientId = _configuration["AWS:ClientId"],
                Username = confirmRegisterationRequest.email,
                ConfirmationCode = confirmRegisterationRequest.confirmationCode,
                Password = confirmRegisterationRequest.newPassword
            };

            try {
                var response = await _provider.ConfirmForgotPasswordAsync(confirmForgotPasswordRequest);

                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK) {
                    return Ok(response);
                } else {
                    return BadRequest();

                }
            } catch (InvalidPasswordException e) {
                return BadRequest(e.Message);

            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        private async Task<IActionResult> HandleRegistrationConfirmation(ConfirmRegisterationRequestDTO confirmRegisterationRequest) {
            if (string.IsNullOrEmpty(confirmRegisterationRequest.username)) {
                return BadRequest(new APIResponse() { errorMessages = new List<string> { "Username is required for registration confirmation." } });
            }

            var confirmSignUpRequest = new ConfirmSignUpRequest {
                ClientId = _configuration["AWS:ClientId"],
                Username = confirmRegisterationRequest.email,
                ConfirmationCode = confirmRegisterationRequest.confirmationCode
            };

            try {
                var response = await _provider.ConfirmSignUpAsync(confirmSignUpRequest);

                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK) {
                    return Ok(response);
                } else {
                    return BadRequest(new APIResponse() { errorMessages = new List<string> { "Error register confirmation" } });
                }
            } catch (CodeMismatchException ex) {
                return BadRequest(new APIResponse() { errorMessages = new List<string> { ex.Message } });

            } catch (Exception ex) {
                return BadRequest(new APIResponse() { errorMessages = new List<string> { ex.Message } });
            }
        }

        [HttpPost("ResendConfirmationCode")]
        public async Task<IActionResult> ResendConfirmationCode([FromBody] ResendConfirmationCode resendRequest) {
            try {
                if (string.IsNullOrEmpty(resendRequest.email)) {
                    return BadRequest("Email is required.");
                }

                var resendConfirmationRequest = new ResendConfirmationCodeRequest {
                    Username = resendRequest.email,
                    ClientId = _configuration["AWS:ClientId"]
                };

                var response = await _provider.ResendConfirmationCodeAsync(resendConfirmationRequest);

                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK) {
                    return Ok("Confirmation code has been resent.");
                } else {
                    return BadRequest("Failed to resend confirmation code.");
                }
            } catch (Exception ex) {
                return BadRequest($"{ex.Message}");
            }
        }

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDTO forgotPasswordRequestDto) {

            if (string.IsNullOrEmpty(forgotPasswordRequestDto.email)) {
                return BadRequest("User not found.");
            }

            var forgotPasswordRequest = new ForgotPasswordRequest {
                ClientId = _configuration["AWS:ClientId"],
                Username = forgotPasswordRequestDto.email
            };
            try {
                var response = await _provider.ForgotPasswordAsync(forgotPasswordRequest);

                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK) {
                    return Ok(response);
                } else {
                    return BadRequest("reset new password failed");
                }
            } catch (InvalidPasswordException ex) {
                return BadRequest(new APIResponse() { errorMessages = new List<string> { ex.Message } });
            } catch (NotAuthorizedException ex) {
                return BadRequest(new APIResponse() { errorMessages = new List<string> { ex.Message } });
            } catch (LimitExceededException ex) {
                return BadRequest(new APIResponse() { errorMessages = new List<string> { ex.Message } });
            } catch (Exception ex) {
                return BadRequest(new APIResponse() { errorMessages = new List<string> { ex.Message } });
            }

        }
    }
}
