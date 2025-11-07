using AppointmentApp.API.Features.Users.Register;
using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentApp.API.Features.Services.GetServices;

public class GetServicesEndPoint:ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/services/getAll", async (IMediator mediator) =>
            {
                var res = await mediator.Send(new ListServicesQuery());
                return res.Success ? Results.Created($"/api/services/getAll", res) : Results.BadRequest(res);
            })
            .WithTags("Services")
            .WithName("GetAllServices");
    }
}