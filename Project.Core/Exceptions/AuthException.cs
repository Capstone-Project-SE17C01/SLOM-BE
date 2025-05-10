using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.CognitoIdentityProvider.Model;
using Microsoft.AspNetCore.Mvc;
using Project.Core.Entities.Business.DTOs;


namespace Project.Core.Exceptions {

    public static class AuthException {
        private static readonly Dictionary<Type, string> ExceptionMap = new()
        {
            { typeof(InvalidPasswordException), "invalidPasswordException" },
            { typeof(TooManyFailedAttemptsException), "limitExceededException" },
            { typeof(UserNotFoundException), "userNotFound" },
            { typeof(UsernameExistsException), "usernameExisted" },
            { typeof(NotAuthorizedException), "notAuthorizedException" },
            { typeof(LimitExceededException), "limitExceededException" },
            { typeof(ExpiredCodeException), "invalidCodeProvided" },
            { typeof(CodeMismatchException), "codeMismatchException" },
            { typeof(InvalidParameterException), "invalidParameter" },
            { typeof(InvalidLambdaResponseException), "invalidLambdaResponse" },
            { typeof(ResourceNotFoundException), "resourceNotFound" },
            { typeof(TooManyRequestsException), "tooManyRequests" },
            { typeof(UserLambdaValidationException), "userLambdaValidation" },
            { typeof(AliasExistsException), "aliasExists" },
            { typeof(InternalErrorException), "internalError" },
            { typeof(PasswordHistoryPolicyViolationException), "passwordHistoryPolicyViolationException" },
            { typeof(Exception), "unknownError" }
    };

        public static IActionResult Resolve(Exception ex) {
            var error = ExceptionMap.TryGetValue(ex.GetType(), out var message)
                ? message
                : "unknownError";

            return new BadRequestObjectResult(new APIResponse {
                errorMessages = new List<string> { error }
            });
        }
    }

}
