using Application.Authentication.Login;
using Application.Authentication.Logout;
using Application.Authentication.Refresh;
using Application.Authentication.Register;
using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Presentation.ResponseHandling;
using Presentation.ResponseHandling.Response;

namespace Presentation.Endpoints;

public class Authentication: ICarterModule
{
    public const string RefreshTokenCookieName = "refreshToken";
    
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup(nameof(Authentication))
            .WithTags(nameof(Authentication))
            .WithOpenApi();
        
        // Register endpoint
        group.MapPost(
            "Register",
            async (HttpContext context, [FromBody] RegisterCommand request, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(request, cancellationToken);

                return await result.MapToResponse(context, cancellationToken);
            })
            .AllowAnonymous()
            .Produces<Ok<ResponseWithData<UserResponseDto>>>()
            .Produces<BadRequest>(StatusCodes.Status400BadRequest)
            .Produces<Conflict>(StatusCodes.Status409Conflict)
            .WithSummary("Registers new application user");

        // Refresh access token endpoint
        group.MapPut(
            "Refresh",
            async (HttpContext context, ISender sender, CancellationToken cancellationToken) =>
            {
                var refreshToken = context.Request.Cookies[RefreshTokenCookieName];

                if (refreshToken is null)
                {
                    return Results.Unauthorized();
                }

                var request = new RefreshCommand(refreshToken);

                var result = await sender.Send(request, cancellationToken);

                return await result.MapToResponse(cancellationToken);
            })
            .AllowAnonymous()
            .Produces<Ok<ResponseWithData<TokenResponse>>>()
            .Produces<BadRequest>(StatusCodes.Status400BadRequest)
            .Produces<UnauthorizedHttpResult>(StatusCodes.Status401Unauthorized)
            .WithSummary("Refreshes access token");

        // Login user endpoint
        group.MapPut(
                "Login",
                async (HttpContext context, [FromBody] LoginCommand request, ISender sender, CancellationToken cancellationToken) => 
                {
                    var result = await sender.Send(request, cancellationToken);

                    return await result.MapToResponse(context, cancellationToken); 
                })
            .AllowAnonymous()
            .Produces<Ok<ResponseWithData<UserResponseDto>>>()
            .Produces<BadRequest>(StatusCodes.Status400BadRequest)
            .WithSummary("Logs existent user in");
        
        // Logout user endpoint
        group.MapPut(
                "Logout",
                async (HttpContext context, ISender sender, CancellationToken cancellationToken) => 
                {
                    var refreshToken = context.Request.Cookies[RefreshTokenCookieName];

                    if (refreshToken is null)
                    {
                        return Results.Unauthorized();
                    }

                    var request = new LogoutCommand(refreshToken);
                    
                    var result = await sender.Send(request, cancellationToken);

                    return await result.MapToResponse(context, cancellationToken); 
                })
            .AllowAnonymous()
            .Produces<Ok<BaseResponse>>()
            .Produces<BadRequest>(StatusCodes.Status400BadRequest)
            .Produces<UnauthorizedHttpResult>(StatusCodes.Status401Unauthorized)
            .WithSummary("Makes refresh token expired");
    }
}