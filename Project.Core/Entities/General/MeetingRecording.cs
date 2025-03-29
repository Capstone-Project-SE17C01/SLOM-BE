using System;
using System.Collections.Generic;

namespace Project.Infrastructure;

public partial class MeetingRecording
{
    public Guid Id { get; set; }

    public Guid MeetingId { get; set; }

    public string StoragePath { get; set; } = null!;

    public int? Duration { get; set; }

    public bool? Processed { get; set; }

    public string? Transcription { get; set; }

    public virtual Meeting Meeting { get; set; } = null!;
}
