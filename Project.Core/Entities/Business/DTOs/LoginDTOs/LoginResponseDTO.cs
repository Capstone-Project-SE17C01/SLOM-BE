namespace Project.Core.Entities.Business.DTOs.LoginDTOs {
    public class LoginResponseDTO {
        public required string IdToken { get; set; }
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
        public string? UserEmail { get; set; }

        public LoginResponseDTO() {
        }

        public LoginResponseDTO(string IdToken, string AccessToken, string RefreshToken, string UserEmail) {
            this.IdToken = IdToken;
            this.AccessToken = AccessToken;
            this.RefreshToken = RefreshToken;
            this.UserEmail = UserEmail;
        }
    }
}
