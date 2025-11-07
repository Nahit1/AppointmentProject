using AppointmentApp.API.Context;
using AppointmentApp.API.Models.Common;
using AppointmentApp.API.Models.Entity;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AppointmentApp.API.Features.Users.Register;

public sealed record RegisterUserCommand(string Email, string PasswordHash, string DisplayName, string Timezone)
    : IRequest<Response<UserRegisteredDto>>;

public sealed record UserRegisteredDto(Guid Id, string Email, string DisplayName, string Timezone);

// public class RegisterHandlerCommandValidator : AbstractValidator<RegisterUserCommand>
// {
//     public RegisterHandlerCommandValidator()
//     {
//         RuleFor(x => x.Email).NotEmpty().WithMessage("Name is required");
//     }
// }

public sealed class RegisterUserHandler(AppointmentDbContext db):IRequestHandler<RegisterUserCommand, Response<UserRegisteredDto>>
{
    public async Task<Response<UserRegisteredDto>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        // basit tekrar kontrolü
        var exists = await db.Users.AnyAsync(u => u.Email == request.Email, cancellationToken);
        if (exists) return Response<UserRegisteredDto>.Fail("Email already exists");
        
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email.Trim(),
            PasswordHash = request.PasswordHash, // NOT: MVP — sonra proper hashing/identity
            DisplayName = request.DisplayName.Trim(),
            Timezone = string.IsNullOrWhiteSpace(request.Timezone) ? "Europe/Istanbul" : request.Timezone,
            CreatedAtUtc = DateTime.UtcNow
        };
        db.Users.Add(user);
        
        await db.SaveChangesAsync(cancellationToken);

        var dto = new UserRegisteredDto(user.Id, user.Email, user.DisplayName, user.Timezone);
        return Response<UserRegisteredDto>.Ok(dto, "User registered");
        
    }
}