using AppointmentApp.API.Context;
using AppointmentApp.API.Models.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AppointmentApp.API.Features.Services.GetServices;
public sealed record ListServicesQuery() : IRequest<Response<List<ServiceListItemDto>>>;
public sealed record ServiceListItemDto(Guid Id, string Name, int DurationMin, int BufferBeforeMin, int BufferAfterMin, decimal? Price, bool IsActive);

public class GetServicesHandler(AppointmentDbContext db): IRequestHandler<ListServicesQuery, Response<List<ServiceListItemDto>>>
{
    public async Task<Response<List<ServiceListItemDto>>> Handle(ListServicesQuery request, CancellationToken cancellationToken)
    {

        var services = await db.Services.Where(x => x.UserId == new Guid("5c5dd5f9-14f9-4641-ba42-91b1b233a23e"))
            .Select(s=> new ServiceListItemDto(
                s.Id, s.Name, s.DurationMin, s.BufferBeforeMin, s.BufferAfterMin, s.Price, s.IsActive))
            .ToListAsync(cancellationToken);

        return Response<List<ServiceListItemDto>>.Ok(services);
    }
}