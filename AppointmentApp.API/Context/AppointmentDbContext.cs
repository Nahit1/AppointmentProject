using AppointmentApp.API.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace AppointmentApp.API.Context;

public class AppointmentDbContext(DbContextOptions<AppointmentDbContext> opts) : DbContext(opts)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<MessageLog> MessageLogs => Set<MessageLog>();
    public DbSet<WaTemplate> WaTemplates => Set<WaTemplate>();
    public DbSet<WaCredentials> WaCredentials => Set<WaCredentials>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(e =>
        {
            e.Property(x => x.Email).IsRequired();
            e.HasIndex(x => x.Email).IsUnique();
        });

        // Services
        modelBuilder.Entity<Service>(e =>
        {
            e.HasIndex(x => x.UserId);
            e.Property(x => x.Price).HasColumnType("numeric(12,2)");
            e.HasOne(x => x.User).WithMany(u => u.Services).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        // Customers
        modelBuilder.Entity<Customer>(e =>
        {
            e.HasIndex(x => x.UserId);
            e.HasIndex(x => new { x.UserId, x.PhoneE164 }).IsUnique();
            e.HasOne(x => x.User).WithMany(u => u.Customers).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        // Appointments
        modelBuilder.Entity<Appointment>(e =>
        {
            e.HasIndex(x => new { x.UserId, x.StartUtc });
            e.HasIndex(x => x.CustomerId);
            e.Property(x => x.Status).HasConversion<short>();
            e.Property(x => x.ServicePriceSnapshot).HasColumnType("numeric(12,2)");
            e.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Service).WithMany(s => s.Appointments).HasForeignKey(x => x.ServiceId);
            e.HasOne(x => x.Customer).WithMany(c => c.Appointments).HasForeignKey(x => x.CustomerId);
        });

        // MessageLog
        modelBuilder.Entity<MessageLog>(e =>
        {
            e.HasIndex(x => x.UserId);
            e.HasIndex(x => x.AppointmentId);
            e.Property(x => x.Channel).HasConversion<short>();
            e.Property(x => x.Status).HasConversion<short>();
            e.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Appointment).WithMany().HasForeignKey(x => x.AppointmentId).OnDelete(DeleteBehavior.SetNull);
        });

        // WaTemplate
        modelBuilder.Entity<WaTemplate>(e =>
        {
            e.HasIndex(x => x.UserId);
            e.HasIndex(x => new { x.UserId, x.TemplateKey, x.Locale }).IsUnique();
            e.Property(x => x.State).HasConversion<short>();
            e.Property(x => x.Buttons).HasColumnType("jsonb");
            e.Property(x => x.ParamsMap).HasColumnType("jsonb");
            e.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        // WaCredentials
        modelBuilder.Entity<WaCredentials>(e =>
        {
            e.HasKey(x => x.UserId);
            e.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        });
    }
}