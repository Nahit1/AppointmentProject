using AppointmentApp.API.Context;
using AppointmentApp.API.Models.Common;
using AppointmentApp.API.Models.Entity;
using FluentValidation;
using MediatR;

namespace AppointmentApp.API.Features.Services.CreateService;

public sealed record CreateServiceCommand(string Name, int DurationMin, int BufferBeforeMin, int BufferAfterMin, decimal? Price, bool IsActive)
    : IRequest<Response<ServiceCreatedDto>>;

public sealed record ServiceCreatedDto(Guid Id);

public class CreateServiceHandlerValidator : AbstractValidator<CreateServiceCommand>
{
    public CreateServiceHandlerValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(120);
        RuleFor(x => x.DurationMin).GreaterThan(0);
        RuleFor(x => x.BufferBeforeMin).GreaterThanOrEqualTo(0);
        RuleFor(x => x.BufferAfterMin).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0).When(x => x.Price.HasValue);
    }
}


public class CreateServiceHandler(AppointmentDbContext db):IRequestHandler<CreateServiceCommand, Response<ServiceCreatedDto>>
{
    public async Task<Response<ServiceCreatedDto>> Handle(CreateServiceCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name)) return Response<ServiceCreatedDto>.Fail("Name is required");
        if (request.DurationMin <= 0) return Response<ServiceCreatedDto>.Fail("DurationMin must be > 0");
        
        var service = new Service
        {
            Id = Guid.NewGuid(),
            UserId = new Guid("5c5dd5f9-14f9-4641-ba42-91b1b233a23e"),
            Name = request.Name.Trim(),
            DurationMin = request.DurationMin,
            BufferBeforeMin = Math.Max(0, request.BufferBeforeMin),
            BufferAfterMin = Math.Max(0, request.BufferAfterMin),
            Price = request.Price,
            IsActive = request.IsActive,
            CreatedAtUtc = DateTime.UtcNow
        };
        db.Services.Add(service);
        await db.SaveChangesAsync(cancellationToken);

        return Response<ServiceCreatedDto>.Ok(new ServiceCreatedDto(service.Id), "Service created");
    }
}