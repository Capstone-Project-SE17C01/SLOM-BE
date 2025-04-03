namespace Project.Infrastructure;

public partial class Identity {
    public string ProviderId { get; set; } = null!;

    public Guid UserId { get; set; }

    public string IdentityData { get; set; } = null!;

    public string Provider { get; set; } = null!;

    public DateTime? LastSignInAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? Email { get; set; }

    public Guid Id { get; set; }

    public virtual User User { get; set; } = null!;
}
