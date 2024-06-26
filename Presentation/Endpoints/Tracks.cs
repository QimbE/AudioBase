using Application.Tracks.CreateTrack;
using Application.Tracks.DeleteTrack;
using Application.Tracks.UpdateTrack;
using Carter;
using Domain.Users;
using Infrastructure.Authentication.Extensions;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Presentation.ResponseHandling;
using Presentation.ResponseHandling.Response;

namespace Presentation.Endpoints;

public class Tracks: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(nameof(Tracks))
            .WithTags(nameof(Tracks))
            .WithOpenApi();

        group.MapPost(
                "CreateTrack",
                async ([FromBody] CreateTrackCommand request, ISender sender, 
                    CancellationToken cancellationToken) =>
                {
                    var result = await sender.Send(request, cancellationToken);
                
                    return await result.MapToResponse(cancellationToken);
                })
            .UserShouldBeAtLeast(Role.CatalogAdmin)
            .Produces<Ok<BaseResponse>>()
            .Produces<BadRequest>(StatusCodes.Status400BadRequest)
            .Produces<ForbidHttpResult>(StatusCodes.Status403Forbidden)
            .Produces<NotFound>(StatusCodes.Status404NotFound)
            .Produces<Conflict>(StatusCodes.Status409Conflict)
            .WithSummary("Creates a new track");
        
        group.MapPut(
                "UpdateTrack",
                async ([FromBody] UpdateTrackCommand request, ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var result = await sender.Send(request, cancellationToken);

                    return await result.MapToResponse(cancellationToken);
                })
            .UserShouldBeAtLeast(Role.CatalogAdmin)
            .Produces<Ok<BaseResponse>>()
            .Produces<BadRequest>(StatusCodes.Status400BadRequest)
            .Produces<ForbidHttpResult>(StatusCodes.Status403Forbidden)
            .Produces<NotFound>(StatusCodes.Status404NotFound)
            .Produces<Conflict>(StatusCodes.Status409Conflict)
            .WithSummary("Updates data of existing track");
        
        group.MapDelete(
                "DeleteTrack",
                async ([FromBody] DeleteTrackCommand request, ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var result = await sender.Send(request, cancellationToken);

                    return await result.MapToResponse(cancellationToken);
                })
            .UserShouldBeAtLeast(Role.CatalogAdmin)
            .Produces<Ok<BaseResponse>>()
            .Produces<BadRequest>(StatusCodes.Status400BadRequest)
            .Produces<ForbidHttpResult>(StatusCodes.Status403Forbidden)
            .Produces<NotFound>(StatusCodes.Status404NotFound)
            .WithSummary("Deletes track");
    }
}