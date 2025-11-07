using AppointmentApp.API.Context;
using AppointmentApp.API.Models.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AppointmentApp.API.Features.Customers.GetCustomers;

public sealed record ListCustomersQuery()
    : IRequest<Response<List<CustomerListItemDto>>>;
public sealed record CustomerListItemDto(Guid Id, string FullName, string PhoneE164, string? Email);

public class GetCustomerHandler(AppointmentDbContext db): IRequestHandler<ListCustomersQuery, Response<List<CustomerListItemDto>>>
{
    public async Task<Response<List<CustomerListItemDto>>> Handle(ListCustomersQuery request, CancellationToken cancellationToken)
    {
        var customers = await db.Customers.Where(c => c.UserId == new Guid("5c5dd5f9-14f9-4641-ba42-91b1b233a23e"))
            .Select(c => new CustomerListItemDto(c.Id, c.FullName, c.PhoneE164, c.Email))
            .ToListAsync(cancellationToken);
        
        return Response<List<CustomerListItemDto>>.Ok(customers);
    }
}