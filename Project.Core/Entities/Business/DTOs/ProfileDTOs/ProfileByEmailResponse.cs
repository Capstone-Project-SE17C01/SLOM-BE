namespace Project.Core.Entities.Business.DTOs.ProfileDTOs {
    public class ProfileByEmailResponse {
        public Guid Id { get; set; }

        public string? Username { get; set; }

        public string? Email { get; set; }

        public Guid? RoleId { get; set; }

        public string? AvatarUrl { get; set; }

        public string? Bio { get; set; }

        public string? Location { get; set; }
   
        public Guid? PreferredLanguageId { get; set; }
   
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
   
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
   
        public string? LanguageCode { get; set; }

        public bool VipUser { get; set; }
    }
}
