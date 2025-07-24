using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text.Json;
using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Extensions.CognitoAuthentication;
using Microsoft.AspNetCore.Mvc;
using Project.Core.Entities.Business.DTOs;
using Project.Core.Entities.Business.DTOs.ChangePasswordDTOs;
using Project.Core.Entities.Business.DTOs.ForgotPasswordDTOs;
using Project.Core.Entities.Business.DTOs.LoginDTOs;
using Project.Core.Entities.Business.DTOs.RegisterDTOs;
using Project.Core.Exceptions;
using Project.Core.Interfaces.IMapper;
using Project.Core.Interfaces.IRepositories;
using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;

namespace Project.API.Controllers {
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
        private readonly HttpClient _httpClient;
        private readonly IBaseMapper<CognitoTokenResponse, LoginResponseDTO> _mapper;


        public AuthController(IConfiguration configuration, IServiceProvider serviceProvider, ApplicationDbContext db, IBaseMapper<CognitoTokenResponse, LoginResponseDTO> mapper) {
            _configuration = configuration;
            _db = db;
            _profileRepository = new ProfileRepository(_db);
            _languageRepository = new LanguageRepository(_db);
            _roleRepository = new RoleRepository(_db);
            _httpClient = new HttpClient();

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
            _mapper = mapper;
        }
        private async Task<CognitoTokenResponse> ExchangeAuthorizationCodeAsync(LoginGoogleRequestDTO request) {
            string clientId = _configuration["AWS:ClientId"]!;
            string grantType = _configuration["AWS:GrantType"]!;
            string tokenEndpoint = _configuration["AWS:TokenEndpoint"]!;


            var form = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("grant_type",    grantType),
            new KeyValuePair<string, string>("client_id",     clientId),
            new KeyValuePair<string, string>("code",          request.Code),
            new KeyValuePair<string, string>("redirect_uri",  request.RedirectUri),
        });

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            try {
                var response = await _httpClient.PostAsync(tokenEndpoint, form);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();

                var tokenResponse = JsonSerializer.Deserialize<CognitoTokenResponse>(json, new JsonSerializerOptions {
                    PropertyNameCaseInsensitive = true
                });
                return tokenResponse ?? throw new Exception("invalidJson");
            }
            catch (Exception) {
                throw new Exception("invalidGrantCode");
            }
        }

        [HttpPost("LoginWithGoogle")]
        public async Task<IActionResult> LoginWithGoogle(LoginGoogleRequestDTO loginGoogleRequest) {

            try {
                var tokenResponse = await ExchangeAuthorizationCodeAsync(loginGoogleRequest);
                var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(tokenResponse.IdToken);
                var googleSub = jwtToken.Claims.First(c => c.Type == "sub").Value;
                var loginResponse = _mapper.MapModel(tokenResponse);
                loginResponse.UserEmail = jwtToken.Claims.First(c => c.Type == "email").Value;

                if (loginResponse == null) {
                    return BadRequest(new APIResponse() { errorMessages = new List<string> { "invalidJson" } });
                }
                else {
                    bool isProfileExist = await _profileRepository.IsExists("email", loginResponse.UserEmail);
                    if (!isProfileExist) {
                        var langCode = loginGoogleRequest.LanguageCode != null ? loginGoogleRequest.LanguageCode.ToLower() : "en";
                        var enLangId = await _languageRepository.GetIdByCodeAsync(langCode);
                        var RoleId = await _roleRepository.GetIdByNameAsync(loginGoogleRequest.Role);

                        var profile = new Core.Entities.General.Profile {
                            Username = loginResponse.UserEmail.Split('@')[0].Trim(),
                            Email = loginResponse.UserEmail.Trim(),
                            RoleId = RoleId,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow,
                            PreferredLanguageId = enLangId,
                            AvatarUrl = "https://avatar.iran.liara.run/public/5",
                            Id = Guid.Parse(googleSub),

                        };
                        try {
                            await _profileRepository.Create(profile);
                        }
                        catch (Exception) {
                            var deleteUserRequest = new AdminDeleteUserRequest {
                                Username = loginResponse.UserEmail,
                                UserPoolId = _configuration["AWS:UserPoolId"]
                            };
                            await _provider.AdminDeleteUserAsync(deleteUserRequest);
                            return BadRequest(new APIResponse() { errorMessages = new List<string> { "errorProfileCreate" } });
                        }
                    }
                }
                return Ok(new APIResponse() { result = loginResponse });

            }
            catch (Exception ex) {
                return AuthException.Resolve(ex);
            }

        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterationRequestDTO registerDTO) {
            try {
                var getUserRequest = new AdminGetUserRequest {
                    Username = registerDTO.Email,
                    UserPoolId = _configuration["AWS:UserPoolId"]
                };

                try {
                    var getUserResponse = await _provider.AdminGetUserAsync(getUserRequest);
                    if (getUserResponse.UserStatus == UserStatusType.UNCONFIRMED) {
                        var deleteUserRequest = new AdminDeleteUserRequest {
                            Username = registerDTO.Email,
                            UserPoolId = _configuration["AWS:UserPoolId"]
                        };
                        await _provider.AdminDeleteUserAsync(deleteUserRequest);
                    }
                    else {
                        return BadRequest(new APIResponse() { errorMessages = new List<string> { "emailUsed" } });
                    }
                }
                catch (UserNotFoundException) {
                    var listResp = await _provider.ListUsersAsync(new ListUsersRequest {
                        UserPoolId = _configuration["AWS:UserPoolId"],
                        Filter = $"email = \"{registerDTO.Email}\"",
                        Limit = 1
                    });
                    if (listResp != null && listResp.Users.Count > 0) {
                        var existing = listResp.Users[0];
                        if (existing.UserStatus == UserStatusType.EXTERNAL_PROVIDER) {
                            return BadRequest(new APIResponse() { errorMessages = new List<string> { "emailRegisteredWithGoogle" } });
                        }
                    }
                }

                #region Create User after checking if user exists
                var signUpRequest = new SignUpRequest {
                    ClientId = _configuration["AWS:ClientId"],
                    Username = registerDTO.Email,
                    Password = registerDTO.Password,
                    UserAttributes = new List<AttributeType>
                    {
                        new AttributeType
                        {
                            Name = "email",
                            Value = registerDTO.Email
                        }
                    }
                };

                var response = await _provider.SignUpAsync(signUpRequest);

                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK) {

                    var langCode = registerDTO.LanguageCode != null ? registerDTO.LanguageCode.ToLower() : "en";
                    var enLangId = await _languageRepository.GetIdByCodeAsync(langCode);
                    var RoleId = await _roleRepository.GetIdByNameAsync(registerDTO.Role);

                    var profile = new Core.Entities.General.Profile {
                        Username = registerDTO.Email.Split('@')[0].Trim(),
                        Email = registerDTO.Email.Trim(),
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
                    }
                    catch (Exception ex) {
                        var deleteUserRequest = new AdminDeleteUserRequest {
                            Username = registerDTO.Email,
                            UserPoolId = _configuration["AWS:UserPoolId"]
                        };

                        await _provider.AdminDeleteUserAsync(deleteUserRequest);
                        return BadRequest(new APIResponse() { errorMessages = new List<string> { ex.Message } });
                    }

                }
                else {
                    return BadRequest(new APIResponse() { errorMessages = new List<string> { "errorProfileCreate" } });
                }
                #endregion
            }
            catch (Exception ex) {
                return AuthException.Resolve(ex);
            }

        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequestDTO loginRequestDTO) {
            var user = new CognitoUser(loginRequestDTO.Email, _configuration["AWS:ClientId"], _userPool, _provider);

            var authRequest = new InitiateSrpAuthRequest {
                Password = loginRequestDTO.Password
            };

            try {
                var authResponse = await user.StartWithSrpAuthAsync(authRequest).ConfigureAwait(false);
                if (authResponse.AuthenticationResult != null) {
                    var loginResponse = new LoginResponseDTO {
                        IdToken = authResponse.AuthenticationResult.IdToken,
                        AccessToken = authResponse.AuthenticationResult.AccessToken,
                        RefreshToken = authResponse.AuthenticationResult.RefreshToken,
                        UserEmail = loginRequestDTO.Email,
                        RoleName = await _profileRepository.GetRoleNameByEmailAsync(loginRequestDTO.Email)
                    };
                    return Ok(new APIResponse() { result = loginResponse });
                }
                else {
                    return BadRequest(new APIResponse() { errorMessages = new List<string> { "unknownError" } });
                }
            }
            catch (Exception ex) {
                return AuthException.Resolve(ex);
            }
        }

        [HttpPost("ConfirmRegistration")]
        public async Task<IActionResult> ConfirmRegistration(ConfirmRegisterationRequestDTO confirmRegisterationRequest) {
            try {
                if (confirmRegisterationRequest.IsPasswordReset && !string.IsNullOrEmpty(confirmRegisterationRequest.NewPassword)) {
                    return await HandlePasswordReset(confirmRegisterationRequest);
                }
                else {
                    return await HandleRegistrationConfirmation(confirmRegisterationRequest);
                }
            }
            catch (Exception ex) {
                return AuthException.Resolve(ex);
            }
        }

        private async Task<IActionResult> HandlePasswordReset(ConfirmRegisterationRequestDTO confirmRegisterationRequest) {
            if (string.IsNullOrEmpty(confirmRegisterationRequest.NewPassword)) {
                return BadRequest(new APIResponse() { errorMessages = new List<string> { "emptynewPassword" } });

            }

            var confirmForgotPasswordRequest = new ConfirmForgotPasswordRequest {
                ClientId = _configuration["AWS:ClientId"],
                Username = confirmRegisterationRequest.Email,
                ConfirmationCode = confirmRegisterationRequest.ConfirmationCode,
                Password = confirmRegisterationRequest.NewPassword
            };

            try {
                var response = await _provider.ConfirmForgotPasswordAsync(confirmForgotPasswordRequest);
                return Ok(response);
            }
            catch (Exception ex) {
                return AuthException.Resolve(ex);
            }
        }

        private async Task<IActionResult> HandleRegistrationConfirmation(ConfirmRegisterationRequestDTO confirmRegisterationRequest) {
            if (string.IsNullOrEmpty(confirmRegisterationRequest.Username)) {
                return BadRequest(new APIResponse() { errorMessages = new List<string> { "emptyUsername" } });
            }

            var confirmSignUpRequest = new ConfirmSignUpRequest {
                ClientId = _configuration["AWS:ClientId"],
                Username = confirmRegisterationRequest.Email,
                ConfirmationCode = confirmRegisterationRequest.ConfirmationCode
            };

            try {
                var response = await _provider.ConfirmSignUpAsync(confirmSignUpRequest);

                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK) {
                    return Ok(response);
                }
                else {
                    return BadRequest(new APIResponse() { errorMessages = new List<string> { "errorRegisterConfirmation" } });
                }
            }
            catch (Exception ex) {
                return AuthException.Resolve(ex);
            }
        }

        [HttpPost("ResendConfirmationCode")]
        public async Task<IActionResult> ResendConfirmationCode([FromBody] ResendConfirmationCode resendRequest) {
            try {
                if (string.IsNullOrEmpty(resendRequest.Email)) {
                    return BadRequest(new APIResponse() { errorMessages = new List<string> { "emptyEmail" } });
                }

                var resendConfirmationRequest = new ResendConfirmationCodeRequest {
                    Username = resendRequest.Email,
                    ClientId = _configuration["AWS:ClientId"]
                };

                var response = await _provider.ResendConfirmationCodeAsync(resendConfirmationRequest);

                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK) {
                    return Ok("successResendConfirmationCode");
                }
                else {
                    return BadRequest(new APIResponse() { errorMessages = new List<string> { "errorResendConfirmationCode" } });
                }
            }
            catch (Exception) {
                return BadRequest(new APIResponse() { errorMessages = new List<string> { "unknownError" } });
            }
        }

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDTO forgotPasswordRequestDto) {

            if (string.IsNullOrEmpty(forgotPasswordRequestDto.Email)) {
                return BadRequest(new APIResponse() { errorMessages = new List<string> { "userNotFound" } });
            }

            var forgotPasswordRequest = new ForgotPasswordRequest {
                ClientId = _configuration["AWS:ClientId"],
                Username = forgotPasswordRequestDto.Email
            };
            try {
                var response = await _provider.ForgotPasswordAsync(forgotPasswordRequest);

                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK) {
                    return Ok(response);
                }
                else {
                    return BadRequest(new APIResponse() { errorMessages = new List<string> { "errorResetPassword" } });
                }
            }
            catch (Exception ex) {
                return AuthException.Resolve(ex);
            }

        }

        [HttpPost("UpdatePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDTO model) {
            try {
                var request = new ChangePasswordRequest {
                    AccessToken = model.AccessToken,
                    PreviousPassword = model.OldPassword,
                    ProposedPassword = model.NewPassword
                };

                var response = await _provider.ChangePasswordAsync(request);

                return Ok(new APIResponse() { result = "passwordUpdated" });
            }
            catch (Exception ex) {
                return AuthException.Resolve(ex);
            }
        }

    }
}
