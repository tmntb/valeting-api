using System.Security.Claims;
using Api.Context;
using Api.Middleware;
using Common.Enums;
using Microsoft.AspNetCore.Http;

namespace Api.Tests.Middleware;

public class ValetingContextMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_AuthenticatedUser_PopulatesValetingContextAndContinues()
    {
        var userId = Guid.NewGuid();
        var valetingContext = new ValetingApiContext();
        var middleware = new ValetingContextMiddleware(valetingContext);
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, "Test User"),
                new Claim(ClaimTypes.Email, "test@example.com"),
                new Claim(ClaimTypes.Role, "ADMIN")
            ], "Bearer"))
        };
        var nextCalled = false;

        await middleware.InvokeAsync(httpContext, _ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        });

        Assert.True(nextCalled);
        Assert.Equal(userId, valetingContext.UserId);
        Assert.Equal("Test User", valetingContext.Name);
        Assert.Equal("test@example.com", valetingContext.Email);
        Assert.Equal(RoleEnum.ADMIN, valetingContext.Role);
    }

    [Fact]
    public async Task InvokeAsync_AnonymousUser_LeavesContextEmptyAndContinues()
    {
        var valetingContext = new ValetingApiContext();
        var middleware = new ValetingContextMiddleware(valetingContext);
        var httpContext = new DefaultHttpContext();
        var nextCalled = false;

        await middleware.InvokeAsync(httpContext, _ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        });

        Assert.True(nextCalled);
        Assert.Null(valetingContext.UserId);
        Assert.Null(valetingContext.Name);
        Assert.Null(valetingContext.Email);
        Assert.Null(valetingContext.Role);
    }
}
