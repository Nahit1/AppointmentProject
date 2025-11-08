using AppointmentApp.API.Context;
using AppointmentApp.API.Models.Common;
using AppointmentApp.API.Models.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AppointmentApp.API.Features.Appointments.CancelAppointment;


public sealed record CancelAppointmentCommand(Guid Id, string? Reason) : IRequest<Response<object>>;

public sealed class CancelAppointmentHandler(AppointmentDbContext db): IRequestHandler<CancelAppointmentCommand, Response<object>>
{
    public async Task<Response<object>> Handle(CancelAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await db.Appointments.FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == new Guid("5c5dd5f9-14f9-4641-ba42-91b1b233a23e"), cancellationToken);
        if (appointment is null) return Response<object>.Fail("Appointment not found");
        if (appointment.Status == AppointmentStatus.Cancelled) return Response<object>.Ok(new { appointment.Id }, "Already cancelled");
        
        appointment.Status = AppointmentStatus.Cancelled;
        appointment.CancelReason = request.Reason;
        appointment.UpdatedAtUtc = DateTime.UtcNow;
        await db.SaveChangesAsync(cancellationToken);

        return Response<object>.Ok(new { appointment.Id }, "Appointment cancelled");
    }
}