using System.ComponentModel.DataAnnotations;
using AppointmentApp.API.Models.Enums;

namespace AppointmentApp.API.Models.Entity;

public sealed class Appointment
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = default!;

    public Guid ServiceId { get; set; }
    public Service Service { get; set; } = default!;

    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = default!;

    // ZAMAN
    public DateTime StartUtc { get; set; }
    public DateTime EndUtc { get; set; }

    // Snapshot alanları
    [MaxLength(120)] public string ServiceNameSnapshot { get; set; } = default!;
    public int ServiceDurationMin { get; set; }
    public decimal? ServicePriceSnapshot { get; set; }

    // Basit konum
    [MaxLength(160)] public string? LocationName { get; set; }

    // Durum/Neden
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Confirmed;
    [MaxLength(200)] public string? CancelReason { get; set; }

    // Messaging / Jobs
    public DateTime? ConfirmedMsgSentAtUtc { get; set; }
    [MaxLength(80)] public string? Reminder24JobId { get; set; }
    public string? LastMsgError { get; set; }

    // Not & audit
    [MaxLength(500)] public string? Notes { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}