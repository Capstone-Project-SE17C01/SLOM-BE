namespace Project.Tests.Helpers.MockServices {
    public class MockCognitoService {
        public Task<string> AuthenticateAsync(string username, string password) {
            return Task.FromResult("mock-cognito-token");
        }

        public Task<bool> ValidateTokenAsync(string token) {
            return Task.FromResult(true);
        }
    }
}
