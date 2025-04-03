namespace Project.Infrastructure;


public partial class SsoProvider {
    public Guid Id { get; set; }

    public string? ResourceId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<SamlProvider> SamlProviders { get; set; } = new List<SamlProvider>();

    public virtual ICollection<SamlRelayState> SamlRelayStates { get; set; } = new List<SamlRelayState>();

    public virtual ICollection<SsoDomain> SsoDomains { get; set; } = new List<SsoDomain>();
}
