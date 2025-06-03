namespace Project.Core.Entities.Business.DTOs.RegisterDTOs {
    public class ConfirmRegisterationRequestDTO {
        public string? Username { get; set; }
        public required string Email { get; set; }
        public required string ConfirmationCode { get; set; }
        public string? NewPassword { get; set; }
        public string? ConfirmNewPassword { get; set; }
        public string? Role { get; set; }
        public bool IsPasswordReset { get; set; } = false;
    }
}
