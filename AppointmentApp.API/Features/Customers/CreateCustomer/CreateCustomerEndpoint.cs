using Carter;
using MediatR;

namespace AppointmentApp.API.Features.Customers.CreateCustomer;

public sealed class CreateCustomerEndpoint:ICarterModule
{
    public sealed class Request
    {
        public string FullName { get; set; } = default!;
        public string PhoneE164 { get; set; } = default!;
        public string? Email { get; set; }
        public string? Note { get; set; }
    }
    
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/customers", async (Request req, IMediator mediator) =>
            {
                var res = await mediator.Send(new CreateCustomerCommand(req.FullName, req.PhoneE164, req.Email, req.Note));
                return res.Success
                    ? Results.Created($"/api/customers/{res.Data!.Id}", res)
                    : Results.BadRequest(res);
            })
            .WithTags("Customers")
            .WithName("CreateCustomer");
    }
}