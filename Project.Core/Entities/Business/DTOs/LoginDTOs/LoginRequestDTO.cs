namespace Project.Core.Entities.Business.DTOs.LoginDTOs {
    public class LoginRequestDTO {
        public required string email { get; set; }
        public required string password { get; set; }
    }
}
