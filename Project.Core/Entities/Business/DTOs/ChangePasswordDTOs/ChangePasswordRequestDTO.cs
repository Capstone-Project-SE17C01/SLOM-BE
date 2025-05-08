namespace Project.Core.Entities.Business.DTOs.ChangePasswordDTOs {
    public class ChangePasswordRequestDTO {
        public required string accessToken { get; set; }
        public required string oldPassword { get; set; }
        public required string newPassword { get; set; }
    }

}
