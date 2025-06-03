namespace Project.Core.Entities.Business.DTOs.LoginDTOs {
    public class LoginRequestDTO {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
