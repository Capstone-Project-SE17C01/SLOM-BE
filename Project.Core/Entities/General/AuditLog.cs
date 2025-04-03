namespace Project.Infrastructure;

public partial class AuditLog {
    public Guid Id { get; set; }

    public string ActionType { get; set; } = null!;

    public Guid? UserId { get; set; }

    public string? TargetTable { get; set; }

    public string? OldValue { get; set; }

    public string? NewValue { get; set; }

    public DateTime? Timestamp { get; set; }

    public virtual Profile? User { get; set; }
}
