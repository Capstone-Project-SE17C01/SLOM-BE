using System;
using System.Collections.Generic;

namespace Project.Infrastructure;

public partial class MeetingParticipant
{
    public Guid MeetingId { get; set; }

    public Guid UserId { get; set; }

    public DateTime? JoinTime { get; set; }

    public DateTime? LeaveTime { get; set; }

    public string? DeviceInfo { get; set; }

    public virtual Meeting Meeting { get; set; } = null!;

    public virtual Profile User { get; set; } = null!;
}
