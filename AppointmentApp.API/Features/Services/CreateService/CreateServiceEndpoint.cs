using Carter;
using MediatR;

namespace AppointmentApp.API.Features.Services.CreateService;

public class CreateServiceEndpoint:ICarterModule
{
    public sealed class Request
    {
        public string Name { get; set; } = default!;
        public int DurationMin { get; set; }
        public int BufferBeforeMin { get; set; }
        public int BufferAfterMin { get; set; }
        public decimal? Price { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/services", async (Request req, IMediator mediator) =>
            {
                var res = await mediator.Send(new CreateServiceCommand(
                    req.Name, req.DurationMin, req.BufferBeforeMin, req.BufferAfterMin, req.Price, req.IsActive));
                return res.Success
                    ? Results.Created($"/api/services/{res.Data!.Id}", res)
                    : Results.BadRequest(res);
            })
            .WithTags("Services")
            .WithName("CreateService");
    }
}