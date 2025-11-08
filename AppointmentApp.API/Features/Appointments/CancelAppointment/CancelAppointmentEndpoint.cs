using Carter;
using MediatR;

namespace AppointmentApp.API.Features.Appointments.CancelAppointment;

public class CancelAppointmentEndpoint:ICarterModule
{
    public sealed class Request { public string? Reason { get; set; } }

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/appointments/{id:guid}/cancel", async (Guid id, Request req, IMediator mediator) =>
            {
                var res = await mediator.Send(new CancelAppointmentCommand(id, req.Reason));
                return res.Success ? Results.Ok(res) : Results.BadRequest(res);
            })
            .WithTags("Appointments")
            .WithName("CancelAppointment");
    }
}