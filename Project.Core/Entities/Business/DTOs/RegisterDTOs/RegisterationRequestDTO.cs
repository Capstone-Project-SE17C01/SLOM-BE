namespace Project.Core.Entities.Business.DTOs.RegisterDTOs {
    public class RegisterationRequestDTO {
        public required string email { get; set; }
        public required string password { get; set; }
        public required string role { get; set; }
    }
}
