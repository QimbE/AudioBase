using Application.Genres.CreateGenre;
using Application.Genres.RenameGenre;
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

public class Genres: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(nameof(Genres))
            .WithTags(nameof(Genres))
            .WithOpenApi();
        
        group.MapPost(
            "CreateGenre",
            async ([FromBody] CreateGenreCommand request, ISender sender, 
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(request, cancellationToken);

                return await result.MapToResponse(cancellationToken);
            })
            .UserShouldBeAtLeast(Role.CatalogAdmin)
            .Produces<Ok<BaseResponse>>()
            .Produces<NotFound>(StatusCodes.Status404NotFound)
            .Produces<BadRequest>(StatusCodes.Status400BadRequest)
            .Produces<Conflict>(StatusCodes.Status409Conflict)
            .WithSummary("Creates a new genre");
        
        group.MapPatch(
            "RenameGenre",
            async ([FromBody] RenameGenreCommand request, ISender sender,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(request, cancellationToken);

                return await result.MapToResponse(cancellationToken);
            })
            .UserShouldBeAtLeast(Role.CatalogAdmin)
            .Produces<Ok<BaseResponse>>()
            .Produces<NotFound>(StatusCodes.Status404NotFound)
            .Produces<BadRequest>(StatusCodes.Status400BadRequest)
            .Produces<Conflict>(StatusCodes.Status409Conflict)
            .WithSummary("Sets new name to existing genre");
    }
}