namespace Project.Core.Entities.Business.DTOs.ChangePasswordDTOs {
    public class ChangePasswordRequestDTO {
        public required string AccessToken { get; set; }
        public required string OldPassword { get; set; }
        public required string NewPassword { get; set; }
    }

}
