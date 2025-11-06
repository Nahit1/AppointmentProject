using System.ComponentModel.DataAnnotations;
using AppointmentApp.API.Models.Enums;

namespace AppointmentApp.API.Models.Entity;

public sealed class MessageLog
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = default!;

    public Guid? AppointmentId { get; set; }
    public Appointment? Appointment { get; set; }

    public MessageChannel Channel { get; set; } = MessageChannel.WhatsApp;

    // confirm / reminder24h / reschedule / cancel
    [MaxLength(64)] public string TemplateKey { get; set; } = default!;

    [MaxLength(20)] public string ToPhoneE164 { get; set; } = default!;
    [MaxLength(8)]  public string Locale { get; set; } = "tr";

    public MessageStatus Status { get; set; } = MessageStatus.Queued;
    [MaxLength(128)] public string? ProviderMessageId { get; set; }
    public string? Error { get; set; }

    public DateTime? SentAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}