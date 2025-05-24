namespace Project.Core.Entities.General {
    public class UserModuleProgress {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid ModuleId { get; set; }

        public DateTime? CompletedAt { get; set; }

        public Profile? User { get; set; }

        public Module? Module { get; set; }

        public bool IsActive { get; set; }
    }
}
