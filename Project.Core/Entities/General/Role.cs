namespace Project.Core.Entities.General {
    public class Role {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Profile> Profiles { get; set; } = new List<Profile>();
    }
}
