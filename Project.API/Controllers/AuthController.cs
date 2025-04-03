﻿using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Extensions.CognitoAuthentication;
using Microsoft.AspNetCore.Mvc;
using Project.Core.Entities.Business.DTOs.ForgotPasswordDTOs;
using Project.Core.Entities.Business.DTOs.LoginDTOs;
using Project.Core.Entities.Business.DTOs.RegisterDTOs;
using System.IdentityModel.Tokens.Jwt;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Project.API.Controllers {
    [Route("api/Auth")]
    [ApiController]
    public class AuthController : ControllerBase {
        private readonly AmazonCognitoIdentityProviderClient _provider;
        private readonly CognitoUserPool _userPool;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;


        public AuthController(IConfiguration configuration, IServiceProvider serviceProvider) {
            _configuration = configuration;
            _serviceProvider = serviceProvider;

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
                    return Ok(registerDTO);

                } else {
                    return BadRequest("registration failed");
                }
                #endregion
            } catch (Exception ex) {
                return BadRequest(ex.Message);
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
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(authResponse.AuthenticationResult.IdToken) as JwtSecurityToken;
                if (authResponse.AuthenticationResult != null) {
                    var loginResponse = new LoginResponseDTO {
                        idToken = authResponse.AuthenticationResult.IdToken,
                        accessToken = authResponse.AuthenticationResult.AccessToken,
                        refreshToken = authResponse.AuthenticationResult.RefreshToken,
                        userEmail = loginRequestDTO.email
                    };
                    return Ok(loginResponse);
                } else {
                    return BadRequest("Authen Failed");
                }
            } catch (Exception ex) {
                return BadRequest(ex.Message);

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
                return BadRequest("Username is required for registration confirmation.");
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
                    return BadRequest("Confirmation failed.");
                }
            } catch (CodeMismatchException) {
                return BadRequest("Invalid confirmation code.");

            } catch (Exception ex) {
                return BadRequest(ex.Message);
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
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest changePasswordRequest) {

            if (string.IsNullOrEmpty(changePasswordRequest.AccessToken)) {
                return BadRequest("User is not authenticated.");
            }

            if (string.IsNullOrEmpty(changePasswordRequest.PreviousPassword)) {
                return BadRequest("Previous Password is required.");
            }

            if (string.IsNullOrEmpty(changePasswordRequest.ProposedPassword)) {
                return BadRequest("New Password is required.");
            }
            try {
                var response = await _provider.ChangePasswordAsync(changePasswordRequest);

                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK) {
                    return Ok(response);
                } else {
                    return BadRequest("Change password failed");
                }
            } catch (InvalidPasswordException ex) {
                return BadRequest(ex.Message);

            } catch (NotAuthorizedException ex) {
                return BadRequest(ex.Message);

            } catch (Exception ex) {

                return BadRequest(ex.Message);
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
                return BadRequest(ex.Message);

            } catch (NotAuthorizedException ex) {
                return BadRequest(ex.Message);

            } catch (Exception ex) {

                return BadRequest(ex.Message);
            }

        }
    }
}




