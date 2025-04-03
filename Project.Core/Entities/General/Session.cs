using System.Net;

namespace Project.Infrastructure;

public partial class Session {
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? FactorId { get; set; }

    public DateTime? NotAfter { get; set; }

    public DateTime? RefreshedAt { get; set; }

    public string? UserAgent { get; set; }

    public IPAddress? Ip { get; set; }

    public string? Tag { get; set; }

    public virtual ICollection<MfaAmrClaim> MfaAmrClaims { get; set; } = new List<MfaAmrClaim>();

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public virtual User User { get; set; } = null!;
}
