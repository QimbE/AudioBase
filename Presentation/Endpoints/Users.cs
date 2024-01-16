using Application.Users.ChangeRole;
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

public class Users: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("User")
            .WithTags("User");

        // Change user role endpoint
        group.MapPatch(
                "ChangeRole",
                async ([FromBody] ChangeRoleCommand request, ISender sender, CancellationToken cancellationToken) =>
                {
                    var result = await sender.Send(request, cancellationToken);

                    return await result.MapToResponse(cancellationToken);
                })
            .Produces<Ok<BaseResponse>>()
            .Produces<NotFound>()
            .Produces<BadRequest>()
            .UserShouldBeAtLeast(Role.Admin)
            .WithSummary("Changes role of specific user");
    }
}