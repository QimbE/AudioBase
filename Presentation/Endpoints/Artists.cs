using Application.Artists.CreateArtist;
using Application.Artists.DeleteArtist;
using Application.Artists.UpdateArtist;
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

public class Artists: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(nameof(Artists))
            .WithTags(nameof(Artists))
            .WithOpenApi();
        
        group.MapPost(
            "CreateArtist",
            async ([FromBody] CreateArtistCommand request, ISender sender, 
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(request, cancellationToken);

                return await result.MapToResponse(cancellationToken);
            })
            .UserShouldBeAtLeast(Role.CatalogAdmin)
            .Produces<Ok<BaseResponse>>()
            .Produces<BadRequest>(StatusCodes.Status400BadRequest)
            .Produces<ForbidHttpResult>(StatusCodes.Status403Forbidden)
            .Produces<Conflict>(StatusCodes.Status409Conflict)
            .WithSummary("Creates a new artist");
        
        group.MapPut(
                "UpdateArtist",
                async ([FromBody] UpdateArtistCommand request, ISender sender,
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
            .WithSummary("Updates data of existing artist");
        
        group.MapDelete(
                "DeleteArtist",
                async ([FromBody] DeleteArtistCommand request, ISender sender,
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
            .WithSummary("Deletes artist");
    }
}