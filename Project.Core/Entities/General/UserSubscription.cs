using System;
using System.Collections.Generic;

namespace Project.Infrastructure;

public partial class UserSubscription
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid PlanId { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? Status { get; set; }

    public virtual SubscriptionPlan Plan { get; set; } = null!;

    public virtual Profile User { get; set; } = null!;
}
