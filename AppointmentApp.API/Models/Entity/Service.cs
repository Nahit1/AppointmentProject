using System.ComponentModel.DataAnnotations;

namespace AppointmentApp.API.Models.Entity;

public sealed class Service
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = default!;

    [MaxLength(120)] public string Name { get; set; } = default!;
    public int DurationMin { get; set; }
    public int BufferBeforeMin { get; set; }
    public int BufferAfterMin { get; set; }
    public decimal? Price { get; set; }
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}