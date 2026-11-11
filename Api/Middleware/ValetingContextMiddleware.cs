using System.Security.Claims;
using Api.Context;
using Common.Enums;

namespace Api.Middleware;

public sealed class ValetingContextMiddleware(ValetingApiContext valetingApiContext) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var identity = context.User.Identities.FirstOrDefault(userIdentity => userIdentity.IsAuthenticated);

        if (identity is not null)
        {
            var userIdClaim = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(userIdClaim, out var userId))
            {
                valetingApiContext.UserId = userId;
            }

            valetingApiContext.Name = identity.FindFirst(ClaimTypes.Name)?.Value;
            valetingApiContext.Email = identity.FindFirst(ClaimTypes.Email)?.Value;

            var roleStr = identity.FindFirst(ClaimTypes.Role)?.Value;
            if (roleStr != null)
            {
                valetingApiContext.Role = Enum.Parse<RoleEnum>(roleStr);
            }
            
            valetingApiContext.ClientIpAddress = context.Connection.RemoteIpAddress?.ToString();
        }

        await next(context);
    }
}
