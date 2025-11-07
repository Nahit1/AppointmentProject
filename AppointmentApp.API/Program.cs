using System.Reflection;
using AppointmentApp.API.Context;
using AppointmentApp.API.Features.Users.Register;
using Carter;
using Hangfire;
using Hangfire.PostgreSql;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCarter();

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));



var connectionString = builder.Configuration.GetConnectionString("Database");

builder.Services.AddHangfire(cfg =>
    cfg.UsePostgreSqlStorage(connectionString));
builder.Services.AddHangfireServer();


builder.Services.AddDbContext<AppointmentDbContext>(opt =>
{
    opt.UseNpgsql(connectionString);
});

var app = builder.Build();

app.UseRouting();
app.UseHangfireDashboard("/hangfire");

// carter routing
app.MapCarter();

app.Run();