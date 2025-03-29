using System;
using System.Collections.Generic;

namespace Project.Infrastructure;

public partial class SubscriptionPlan
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal? Price { get; set; }

    public int? DurationDays { get; set; }

    public string? Features { get; set; }

    public virtual ICollection<UserSubscription> UserSubscriptions { get; set; } = new List<UserSubscription>();
}
