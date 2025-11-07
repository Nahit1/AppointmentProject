using AppointmentApp.API.Features.Services.GetServices;
using Carter;
using MediatR;

namespace AppointmentApp.API.Features.Customers.GetCustomers;

public class GetCustomerEndPoint:ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/custormer/getAll", async (IMediator mediator) =>
            {
                var res = await mediator.Send(new ListCustomersQuery());
                return res.Success ? Results.Created($"/api/custormer/getAll", res) : Results.BadRequest(res);
            })
            .WithTags("Customers")
            .WithName("GetAllCustomers");
    }
}