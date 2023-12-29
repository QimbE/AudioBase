using Domain.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Authentication.Extensions;

public static class RolePoliciesExtensions
{
    private static AuthorizationBuilder AddRoleAdminPolicy(this AuthorizationBuilder builder)
    {
        return builder.AddPolicy(Role.Admin.Name, policy =>
        {
            policy.RequireRole(Role.Admin.Name);
        });
    }

    private static AuthorizationBuilder AddRoleDataBaseAdminPolicy(this AuthorizationBuilder builder)
    {
        return builder.AddPolicy(Role.CatalogAdmin.Name, policy =>
        {
            policy.RequireRole(Role.Admin.Name, Role.CatalogAdmin.Name);
        });
    }
    
    private static AuthorizationBuilder AddRoleDefaultUserPolicy(this AuthorizationBuilder builder)
    {
        return builder.AddPolicy(Role.DefaultUser.Name, policy =>
        {
            policy.RequireRole(Role.Admin.Name, Role.CatalogAdmin.Name, Role.DefaultUser.Name);
        });
    }
    
    public static AuthorizationBuilder AddRoleAuthorizationPolicies(this IServiceCollection services)
    {
        return services.AddAuthorizationBuilder()
            .AddRoleDefaultUserPolicy()
            .AddRoleDataBaseAdminPolicy()
            .AddRoleAdminPolicy();
    }
}