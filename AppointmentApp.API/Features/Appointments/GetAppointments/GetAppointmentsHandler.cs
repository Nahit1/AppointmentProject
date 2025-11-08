using AppointmentApp.API.Context;
using AppointmentApp.API.Models.Common;
using AppointmentApp.API.Models.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AppointmentApp.API.Features.Appointments.GetAppointments;

public sealed record ListAppointmentsQuery(DateTime? FromUtc, DateTime? ToUtc, Guid? CustomerId)
    : IRequest<Response<List<AppointmentListItemDto>>>;

public sealed record AppointmentListItemDto(
    Guid Id, DateTime StartUtc, DateTime EndUtc, AppointmentStatus Status,
    string ServiceName, int ServiceDurationMin,
    Guid CustomerId, string CustomerFullName, string CustomerPhoneE164
);

public sealed class GetAppointmentsHandler(AppointmentDbContext db):IRequestHandler<ListAppointmentsQuery, Response<List<AppointmentListItemDto>>>
{
    public async Task<Response<List<AppointmentListItemDto>>> Handle(ListAppointmentsQuery request, CancellationToken cancellationToken)
    {
        var query = db.Appointments.AsNoTracking()
            .Where(a => a.UserId == new Guid("5c5dd5f9-14f9-4641-ba42-91b1b233a23e"));
        
        if (request.FromUtc.HasValue) query = query.Where(a => a.StartUtc >= request.FromUtc.Value);
        if (request.ToUtc.HasValue)   query = query.Where(a => a.StartUtc <  request.ToUtc.Value);
        if (request.CustomerId.HasValue) query = query.Where(a => a.CustomerId == request.CustomerId.Value);
        
        var appointmentList = await query.OrderBy(a => a.StartUtc)
            .Select(a => new AppointmentListItemDto(
                a.Id, a.StartUtc, a.EndUtc, a.Status,
                a.ServiceNameSnapshot, a.ServiceDurationMin,
                a.CustomerId, a.Customer.FullName, a.Customer.PhoneE164))
            .ToListAsync(cancellationToken);
        
        return Response<List<AppointmentListItemDto>>.Ok(appointmentList);
    }
}