namespace Project.Core.Entities.General {
    public class SubscriptionPlan {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public decimal Price { get; set; }

        public int DurationDays { get; set; }

        public string? Description { get; set; }

        public string? Features { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<UserSubscription> UserSubscriptions { get; set; } = new List<UserSubscription>();
    }
}
