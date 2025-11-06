using System.ComponentModel.DataAnnotations;
using AppointmentApp.API.Models.Enums;

namespace AppointmentApp.API.Models.Entity;

public sealed class WaTemplate
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = default!;

    [MaxLength(80)]  public string Name { get; set; } = default!;
    [MaxLength(64)]  public string TemplateKey { get; set; } = default!; // confirm / reminder24h / reschedule / cancel
    [MaxLength(24)]  public string Category { get; set; } = "UTILITY";
    [MaxLength(8)]   public string Locale { get; set; } = "tr";

    [MaxLength(60)]  public string? HeaderText { get; set; }
    public string BodyText { get; set; } = default!;
    [MaxLength(60)]  public string? FooterText { get; set; }

    // JSON sütunları
    public List<WaTemplateButton> Buttons { get; set; } = new();
    public Dictionary<string,string> ParamsMap { get; set; } = new();

    public WaTemplateState State { get; set; } = WaTemplateState.Draft;
    [MaxLength(128)] public string? ProviderTemplateId { get; set; }
    public string? ReviewNote { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}
