using AppointmentApp.API.Context;
using AppointmentApp.API.Models.Common;
using AppointmentApp.API.Models.Entity;
using AppointmentApp.API.Models.Enums;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AppointmentApp.API.Features.Appointments.CreateAppointment;

public sealed record CreateAppointmentCommand(
    Guid ServiceId,
    string Date,   // "yyyy-MM-dd" local (Europe/Istanbul)
    string Time,   // "HH:mm"     local
    Guid? CustomerId,
    string? CustomerFullName,
    string? CustomerPhoneE164,
    string? LocationName,
    string? Notes
) : IRequest<Response<AppointmentCreatedDto>>;

public sealed record AppointmentCreatedDto(Guid Id, DateTime StartUtc);


public class CreateAppointmentHandlerValidator : AbstractValidator<CreateAppointmentCommand>
{
    public CreateAppointmentHandlerValidator()
    {
        RuleFor(x => x.ServiceId).NotEmpty();
        RuleFor(x => x.Date).NotEmpty().Matches(@"^\d{4}-\d{2}-\d{2}$");
        RuleFor(x => x.Time).NotEmpty().Matches(@"^\d{2}:\d{2}$");

        // Customer: Id varsa detay zorunlu değil; yoksa ad + telefon zorunlu
        When(x => !x.CustomerId.HasValue, () =>
        {
            RuleFor(x => x.CustomerFullName).NotEmpty().MaximumLength(160);
            RuleFor(x => x.CustomerPhoneE164)
                .NotEmpty()
                .Matches(@"^\+\d{8,16}$");
        });

        RuleFor(x => x.LocationName).MaximumLength(160).When(x => !string.IsNullOrWhiteSpace(x.LocationName));
        RuleFor(x => x.Notes).MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.Notes));
    }
}

public sealed class CreateAppointmentHandler(AppointmentDbContext db): IRequestHandler<CreateAppointmentCommand, Response<AppointmentCreatedDto>>
{
    public async Task<Response<AppointmentCreatedDto>> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
    {
        // service
        var service = await db.Services.FirstOrDefaultAsync(x => x.Id == new Guid("0e41c246-80a7-42cd-94e9-5ea6dcdad875") && x.UserId == new Guid("5c5dd5f9-14f9-4641-ba42-91b1b233a23e"), cancellationToken);
        if (service is null) return Response<AppointmentCreatedDto>.Fail("Service not found");
        
        var customer = request.CustomerId.HasValue
            ? await db.Customers.FirstOrDefaultAsync(x => x.Id == request.CustomerId && x.UserId == new Guid("5c5dd5f9-14f9-4641-ba42-91b1b233a23e"), cancellationToken)
            : null;

        if (customer is null)
        {
            if (string.IsNullOrWhiteSpace(request.CustomerFullName) || string.IsNullOrWhiteSpace(request.CustomerPhoneE164))
                return Response<AppointmentCreatedDto>.Fail("Customer info required");
            
            var phone = request.CustomerPhoneE164.Trim();
            
            customer = await db.Customers.FirstOrDefaultAsync(x => x.UserId == new Guid("5c5dd5f9-14f9-4641-ba42-91b1b233a23e") && x.PhoneE164 == phone, cancellationToken);
            
            if (customer is null)
            {
                customer = new Customer {
                    Id = Guid.NewGuid(), 
                    UserId = new Guid("5c5dd5f9-14f9-4641-ba42-91b1b233a23e"),
                    FullName = request.CustomerFullName!.Trim(),
                    PhoneE164 = phone,
                    CreatedAtUtc = DateTime.UtcNow
                };
                db.Customers.Add(customer);
                await db.SaveChangesAsync(cancellationToken);
            }
        }
        
        // local -> UTC
        if (!DateTime.TryParse($"{request.Date} {request.Time}", out var localStart))
            return Response<AppointmentCreatedDto>.Fail("Invalid date/time");
        
        var tz = TimeZoneInfo.FindSystemTimeZoneById("Europe/Istanbul"); // TODO: user.Timezone
        var startUtc = TimeZoneInfo.ConvertTimeToUtc(localStart, tz);
        var endUtc = startUtc.AddMinutes(service.DurationMin + service.BufferAfterMin);
        
        // conflict
        var conflict = await db.Appointments.AnyAsync(a =>
            a.UserId == new Guid("5c5dd5f9-14f9-4641-ba42-91b1b233a23e") &&
            a.Status != AppointmentStatus.Cancelled &&
            a.StartUtc < endUtc && startUtc < a.EndUtc, cancellationToken);
        if (conflict) return Response<AppointmentCreatedDto>.Fail("Slot conflict");
        
        
        var aEnt = new Appointment
        {
            Id = Guid.NewGuid(),
            UserId = new Guid("5c5dd5f9-14f9-4641-ba42-91b1b233a23e"),
            ServiceId = service.Id,
            CustomerId = customer.Id,
            StartUtc = startUtc,
            EndUtc = endUtc,
            Status = AppointmentStatus.Confirmed,
            ServiceNameSnapshot = service.Name,
            ServiceDurationMin = service.DurationMin,
            ServicePriceSnapshot = service.Price,
            LocationName = request.LocationName,
            Notes = request.Notes,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        db.Appointments.Add(aEnt);
        await db.SaveChangesAsync(cancellationToken);
        
        return Response<AppointmentCreatedDto>.Ok(new(aEnt.Id, aEnt.StartUtc), "Appointment created");
    }
}