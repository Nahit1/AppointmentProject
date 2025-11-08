using Carter;
using MediatR;

namespace AppointmentApp.API.Features.Appointments.GetAppointments;

public class GetAppointmentEndpoint:ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/appointments", async (DateTime? fromUtc, DateTime? toUtc, Guid? customerId, IMediator mediator) =>
            {
                var res = await mediator.Send(new ListAppointmentsQuery(fromUtc, toUtc, customerId));
                return Results.Ok(res);
            })
            .WithTags("Appointments")
            .WithName("ListAppointments");
    }
}