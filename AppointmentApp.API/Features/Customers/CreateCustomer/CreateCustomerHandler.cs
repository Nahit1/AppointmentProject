using AppointmentApp.API.Context;
using AppointmentApp.API.Models.Common;
using AppointmentApp.API.Models.Entity;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AppointmentApp.API.Features.Customers.CreateCustomer;

public sealed record CreateCustomerCommand(string FullName, string PhoneE164, string? Email, string? Note)
    : IRequest<Response<CustomerCreatedDto>>;

public sealed record CustomerCreatedDto(Guid Id);

public class CreateCustomerHandlerValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerHandlerValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(160);
        RuleFor(x => x.PhoneE164)
            .NotEmpty()
            .Matches(@"^\+\d{8,16}$").WithMessage("Phone must be E.164 format, e.g. +905xxxxxxxxx");
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.Note).MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.Note));
    }
}


public sealed class CreateCustomerHandler(AppointmentDbContext db): IRequestHandler<CreateCustomerCommand, Response<CustomerCreatedDto>>
{
    public async Task<Response<CustomerCreatedDto>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.FullName)) return Response<CustomerCreatedDto>.Fail("FullName required");
        if (string.IsNullOrWhiteSpace(request.PhoneE164)) return Response<CustomerCreatedDto>.Fail("Phone required");
        
        var exists = await db.Customers
            .AnyAsync(x => 
                x.UserId == new Guid("5c5dd5f9-14f9-4641-ba42-91b1b233a23e") && 
                x.PhoneE164 == request.PhoneE164.Trim(), cancellationToken);
        
        if (exists) return Response<CustomerCreatedDto>.Fail("Phone already exists");
        
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            UserId = new Guid("5c5dd5f9-14f9-4641-ba42-91b1b233a23e"),
            FullName = request.FullName.Trim(),
            PhoneE164 = request.PhoneE164.Trim(),
            Email = request.Email?.Trim(),
            Note = request.Note,
            CreatedAtUtc = DateTime.UtcNow
        };
        db.Customers.Add(customer);
        await db.SaveChangesAsync(cancellationToken);
        
        return Response<CustomerCreatedDto>.Ok(new(customer.Id), "Customer created");
    }
}