using Carter;
using MediatR;

namespace AppointmentApp.API.Features.Appointments.CreateAppointment;

public class CreateAppointmentEndpoint:ICarterModule
{
    public sealed class Request
    {
        public Guid ServiceId { get; set; }
        public string Date { get; set; } = default!; // local "yyyy-MM-dd"
        public string Time { get; set; } = default!; // local "HH:mm"
        public Guid? CustomerId { get; set; }
        public string? CustomerFullName { get; set; }
        public string? CustomerPhoneE164 { get; set; }
        public string? LocationName { get; set; }
        public string? Notes { get; set; }
    }

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/appointments", async (Request req, IMediator mediator) =>
            {
                var res = await mediator.Send(new CreateAppointmentCommand(
                    req.ServiceId, req.Date, req.Time,
                    req.CustomerId, req.CustomerFullName, req.CustomerPhoneE164,
                    req.LocationName, req.Notes));
                return res.Success
                    ? Results.Created($"/api/appointments/{res.Data!.Id}", res)
                    : Results.BadRequest(res);
            })
            .WithTags("Appointments")
            .WithName("CreateAppointment");
    }
}