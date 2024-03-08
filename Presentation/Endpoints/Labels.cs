using Application.Labels.CreateLabel;
using Application.Labels.UpdateLabel;
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

public class Labels: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(nameof(Labels))
            .WithTags(nameof(Labels))
            .WithOpenApi();

        group.MapPost(
                "CreateLabel",
                async ([FromBody] CreateLabelCommand request, ISender sender,
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
            .WithSummary("Creates a new label");
        
        group.MapPut(
                "UpdateLabel",
                async ([FromBody] UpdateLabelCommand request, ISender sender,
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
            .WithSummary("Updates data of existing label");
    }
}