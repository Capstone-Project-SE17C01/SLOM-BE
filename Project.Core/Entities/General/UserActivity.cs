using System;
using System.Collections.Generic;

namespace Project.Infrastructure;

public partial class UserActivity
{
    public Guid Id { get; set; }

    public Guid? UserId { get; set; }

    public string ActivityType { get; set; } = null!;

    public Guid? TargetId { get; set; }

    public string? Metadata { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Profile? User { get; set; }
}
