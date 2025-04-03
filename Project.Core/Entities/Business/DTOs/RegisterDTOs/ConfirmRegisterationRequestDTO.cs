namespace Project.Core.Entities.Business.DTOs.RegisterDTOs
{
    public class ConfirmRegisterationRequestDTO
    {
        public string? username { get; set; }
        public string email { get; set; }
        public string confirmationCode { get; set; }
        public string? newPassword { get; set; }
        public string? confirmNewPassword { get; set; }
        public string? role { get; set; }
        public bool isPasswordReset { get; set; } = false;

    }
}
