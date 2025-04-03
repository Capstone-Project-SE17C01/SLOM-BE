namespace Project.Infrastructure;

public partial class MfaAmrClaim {
    public Guid SessionId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string AuthenticationMethod { get; set; } = null!;

    public Guid Id { get; set; }

    public virtual Session Session { get; set; } = null!;
}
