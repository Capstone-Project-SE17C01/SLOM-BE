namespace Project.Core.Entities.Business.DTOs.LoginDTOs {
    public class LoginResponseDTO {
        public required string IdToken { get; set; }
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
        public string? UserEmail { get; set; }
        public string? RoleName { get; set; }

        public LoginResponseDTO() {
        }

        public LoginResponseDTO(string IdToken, string AccessToken, string RefreshToken, string UserEmail, string RoleName) {
            this.IdToken = IdToken;
            this.AccessToken = AccessToken;
            this.RefreshToken = RefreshToken;
            this.UserEmail = UserEmail;
            this.RoleName = RoleName;
        }
    }
}
