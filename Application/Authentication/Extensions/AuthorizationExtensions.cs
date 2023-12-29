using Domain.Users;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Application.Authentication.Extensions;

public static class AuthorizationExtensions
{
    public static RouteHandlerBuilder UserShouldBeAtLeast(this RouteHandlerBuilder builder, Role role)
    {
        return builder.RequireAuthorization(role.Name);
    }

    public static RouteGroupBuilder UserShouldBeAtLeast(this RouteGroupBuilder builder, Role role)
    {
        return builder.RequireAuthorization(role.Name);
    }
}