namespace Project.Core.Entities.Business.DTOs.ProfileDTOs {
    public class ProfileUpdateModel {
        public Guid Id { get; set; }

        public string? Username { get; set; }

        public string? Email { get; set; }

        public string? Bio { get; set; }

        public string? AvatarUrl { get; set; }

        public string? Location { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
