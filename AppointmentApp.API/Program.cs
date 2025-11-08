using System.Reflection;
using AppointmentApp.API.Behaviors;
using AppointmentApp.API.Context;
using AppointmentApp.API.Features.Users.Register;
using AppointmentApp.API.Middleware;
using Carter;
using FluentValidation;
using Hangfire;
using Hangfire.PostgreSql;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCarter();

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

// FluentValidation: bu assembly’deki tüm validator’ları tara
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

// MediatR pipeline: validation
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

var connectionString = builder.Configuration.GetConnectionString("Database");

builder.Services.AddHangfire(cfg =>
    cfg.UsePostgreSqlStorage(connectionString));
builder.Services.AddHangfireServer();


builder.Services.AddDbContext<AppointmentDbContext>(opt =>
{
    opt.UseNpgsql(connectionString);
});

var app = builder.Build();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseRouting();
app.UseHangfireDashboard("/hangfire");

// carter routing
app.MapCarter();

app.Run();