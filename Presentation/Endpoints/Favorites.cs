using System.Security.Claims;
using Application.Favorites.CreateFavorite;
using Application.Favorites.DeleteFavorite;
using Carter;
using Domain.Users;
using Infrastructure.Authentication.Extensions;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Presentation.ResponseHandling;
using Presentation.ResponseHandling.Response;

namespace Presentation.Endpoints;

public class Favorites : ICarterModule
{
    public const string RefreshTokenCookieName = "refreshToken";
    
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(nameof(Favorites))
            .WithTags(nameof(Favorites))
            .WithOpenApi();

        group.MapPost(
                "AddFavorite",
                async (HttpContext context, [FromQuery] Guid trackId, ISender sender
                    , CancellationToken cancellationToken) =>
                {
                    var userClaim = context.User.Claims.FirstOrDefault(claim =>claim.Type == ClaimTypes.NameIdentifier);
                    
                    if (userClaim is null)
                    {
                        return Results.Unauthorized();
                    }
                    
                    var request = new CreateFavoriteCommand(new Guid(userClaim.Value), trackId);
                    
                    var result = await sender.Send(request, cancellationToken);

                    return await result.MapToResponse(cancellationToken);
                })
            .UserShouldBeAtLeast(Role.DefaultUser)
            .Produces<Ok<BaseResponse>>()
            .Produces<BadRequest>(StatusCodes.Status400BadRequest)
            .Produces<UnauthorizedHttpResult>(StatusCodes.Status401Unauthorized)
            .Produces<NotFound>(StatusCodes.Status404NotFound)
            .Produces<Conflict>(StatusCodes.Status409Conflict)
            .WithSummary("Adds a track to favorites");
        
        group.MapDelete(
                "DeleteFavorite",
                async (HttpContext context, [FromQuery] Guid trackId, ISender sender
                    , CancellationToken cancellationToken) =>
                {
                    var userClaim = context.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);

                    if (userClaim is null)
                    {
                        return Results.Unauthorized();
                    }
                    
                    var request = new DeleteFavoriteCommand(new Guid(userClaim.Value), trackId);
                    
                    var result = await sender.Send(request, cancellationToken);

                    return await result.MapToResponse(cancellationToken);
                })
            .UserShouldBeAtLeast(Role.DefaultUser)
            .Produces<Ok<BaseResponse>>()
            .Produces<BadRequest>(StatusCodes.Status400BadRequest)
            .Produces<UnauthorizedHttpResult>(StatusCodes.Status401Unauthorized)
            .Produces<NotFound>(StatusCodes.Status404NotFound)
            .Produces<Conflict>(StatusCodes.Status409Conflict)
            .WithSummary("Adds a track to favorites");
    }
}