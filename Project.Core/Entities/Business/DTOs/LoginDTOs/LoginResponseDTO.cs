namespace Project.Core.Entities.Business.DTOs.LoginDTOs {
    public class LoginResponseDTO {
        public string idToken { get; set; }
        public string accessToken { get; set; }
        public string refreshToken { get; set; }
        public string userEmail { get; set; }

        public LoginResponseDTO() {
        }

        public LoginResponseDTO(string idToken, string accessToken, string refreshToken, string userEmail) {
            this.idToken = idToken;
            this.accessToken = accessToken;
            this.refreshToken = refreshToken;
            this.userEmail = userEmail;
        }
    }
}
