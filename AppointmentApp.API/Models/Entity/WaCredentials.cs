using System.ComponentModel.DataAnnotations;

namespace AppointmentApp.API.Models.Entity;

public sealed class WaCredentials
{
    public Guid UserId { get; set; }
    public User User { get; set; } = default!;

    [MaxLength(64)] public string WabaId { get; set; } = default!;
    [MaxLength(64)] public string BusinessPhoneNumberId { get; set; } = default!;
    public string AccessTokenEnc { get; set; } = default!; // encrypted at rest
    [MaxLength(8)]  public string DefaultLocale { get; set; } = "tr";

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}