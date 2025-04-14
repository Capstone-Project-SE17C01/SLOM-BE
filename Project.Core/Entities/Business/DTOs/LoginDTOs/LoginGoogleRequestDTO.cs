namespace Project.Core.Entities.Business.DTOs.LoginDTOs {
    public class LoginGoogleRequestDTO {
        public required string code { get; set; }
        public required string redirectUri { get; set; }
        public required string role { get; set; }
        public string? languageCode { get; set; }
    }
}
