using System;
using System.Collections.Generic;

namespace Project.Infrastructure;

public partial class Meeting
{
    public Guid Id { get; set; }

    public Guid HostId { get; set; }

    public string? Title { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public string? Status { get; set; }

    public string? GuestCode { get; set; }

    public bool? AllowGuests { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Profile Host { get; set; } = null!;

    public virtual ICollection<MeetingParticipant> MeetingParticipants { get; set; } = new List<MeetingParticipant>();

    public virtual ICollection<MeetingRecording> MeetingRecordings { get; set; } = new List<MeetingRecording>();

    public virtual ICollection<Translation> Translations { get; set; } = new List<Translation>();
}
