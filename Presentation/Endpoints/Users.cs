using System.Security.Claims;
using Application.Users.ChangePassword;
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
            .WithTags("User")
            .WithOpenApi();

        // Change user role endpoint
        group.MapPatch(
                "ChangeRole",
                async ([FromBody] ChangeRoleCommand request, ISender sender, CancellationToken cancellationToken) =>
                {
                    var result = await sender.Send(request, cancellationToken);

                    return await result.MapToResponse(cancellationToken);
                })
            .Produces<Ok<BaseResponse>>()
            .Produces<NotFound>(StatusCodes.Status404NotFound)
            .Produces<BadRequest>(StatusCodes.Status400BadRequest)
            .UserShouldBeAtLeast(Role.Admin)
            .WithSummary("Changes role of specific user");
        
        // Change password, when the user is logged in
        group.MapPatch(
            "ChangePassword",
            async (HttpContext context, [FromBody] ChangePasswordRequest request, ISender sender, CancellationToken cancellationToken) =>
            {
                var idClaim = context.User.Claims.FirstOrDefault(claim =>claim.Type == ClaimTypes.NameIdentifier);

                if (idClaim is null)
                {
                    return Results.Unauthorized();
                }

                var result = await sender.Send(
                    request.ToCommand(Guid.Parse(idClaim.Value)),
                    cancellationToken
                    );

                return await result.MapToResponse(cancellationToken);
            })
            .UserShouldBeAtLeast(Role.DefaultUser)
            .Produces<Ok<BaseResponse>>()
            .Produces<NotFound>(StatusCodes.Status404NotFound)
            .Produces<BadRequest>(StatusCodes.Status400BadRequest)
            .Produces<Conflict>(StatusCodes.Status409Conflict)
            .WithSummary("Changes user's password if the user logged in");
    }
}

public record ChangePasswordRequest(string OldPassword, string NewPassword)
{
    public ChangePasswordCommand ToCommand(Guid userId)
    {
        return new ChangePasswordCommand(userId, OldPassword, NewPassword);
    }
}