using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using AppointmentApp.API.Models.Common;

namespace AppointmentApp.API.Middleware;

public sealed class ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
{
    public async Task Invoke(HttpContext ctx)
    {
        try
        {
            await next(ctx);
        }
        catch (ValidationException vex)
        {
            logger.LogWarning(vex, "Validation error");
            var dict = vex.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            ctx.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await ctx.Response.WriteAsJsonAsync(Response<object>.Fail("Validation Failed",dict));
        }
        catch (KeyNotFoundException kex)
        {
            logger.LogInformation(kex, "Not found");
            ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
            await ctx.Response.WriteAsJsonAsync(Response<object>.Fail(kex.Message));
        }
        catch (UnauthorizedAccessException uex)
        {
            logger.LogWarning(uex, "Unauthorized");
            ctx.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await ctx.Response.WriteAsJsonAsync(Response<object>.Fail("Unauthorized"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled error");
            ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await ctx.Response.WriteAsJsonAsync(Response<object>.Fail("Unexpected error"));
        }
    }
}