namespace Project.Core.Entities.Business.DTOs.LoginDTOs {
    public class LoginGoogleRequestDTO {
        public required string Code { get; set; }
        public required string RedirectUri { get; set; }
        public required string Role { get; set; }
        public string? LanguageCode { get; set; }
    }
}
