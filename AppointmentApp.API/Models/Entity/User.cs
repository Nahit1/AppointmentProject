using System.ComponentModel.DataAnnotations;

namespace AppointmentApp.API.Models.Entity;

public sealed class User
{
    public Guid Id { get; set; }
    [MaxLength(200)] public string Email { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    [MaxLength(120)] public string DisplayName { get; set; } = default!;
    [MaxLength(64)]  public string Timezone { get; set; } = "Europe/Istanbul";
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public ICollection<Service> Services { get; set; } = new List<Service>();
    public ICollection<Customer> Customers { get; set; } = new List<Customer>();
}