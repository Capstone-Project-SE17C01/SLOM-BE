namespace Project.Core.Entities.General {
    public class Payment {
        public Guid Id { get; set; }

        public Guid? UserId { get; set; }

        public Guid? SubscriptionId { get; set; }

        public decimal Amount { get; set; }

        public string? Currency { get; set; }

        public string? PaymentMethod { get; set; }

        public string? Status { get; set; }

        public string? TransactionId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Profile User { get; set; } = null!;
        public UserSubscription Subscription { get; set; } = null!;
    }
}
