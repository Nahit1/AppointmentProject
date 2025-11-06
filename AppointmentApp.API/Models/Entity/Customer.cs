using System.ComponentModel.DataAnnotations;

namespace AppointmentApp.API.Models.Entity;

public sealed class Customer
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = default!;

    [MaxLength(160)] public string FullName { get; set; } = default!;
    [MaxLength(20)]  public string PhoneE164 { get; set; } = default!; // +905xx...
    [MaxLength(160)] public string? Email { get; set; }
    [MaxLength(500)] public string? Note { get; set; }
    public bool OptInWhatsApp { get; set; } = true;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}