using System.Data;
using System.Security.Authentication;
using Application.Authentication.Register;
using Domain.Users.Exceptions;
using FluentValidation;
using LanguageExt.Common;
using Microsoft.AspNetCore.Http;
using Presentation.Endpoints;
using Presentation.ResponseHandling.Response;
using Throw;

namespace Presentation.ResponseHandling;

// TODO: Split this into classes
/// <summary>
/// Maps Result to valid API response
/// </summary>
public static class ResultMapper
{
    /// <summary>
    /// Maps Result to valid API response
    /// </summary>
    /// <param name="resultToMap">Some handler result</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <typeparam name="T">Some response type</typeparam>
    /// <returns>Related result</returns>
    public static Task<IResult> MapToResponse<T>(this Result<T> resultToMap, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromCanceled<IResult>(cancellationToken);
        }
        
        return Task.FromResult(
            resultToMap.Match(
                success => success.MapToResponse(),
                failure => failure.MapToResponse()
                )
            );
    }

    /// <summary>
    /// Maps Result to valid API response
    /// </summary>
    /// <param name="resultToMap">Some handler result</param>
    /// <param name="context">Http context</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <typeparam name="T">Some response type</typeparam>
    /// <returns>Related result</returns>
    public static Task<IResult> MapToResponse<T>(this Result<T> resultToMap, HttpContext context, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromCanceled<IResult>(cancellationToken);
        }
        
        return Task.FromResult(
            resultToMap.Match(
                success => success.MapToResponse(context),
                failure => failure.MapToResponse()
            )
        );
    }

    /// <summary>
    /// Maps any succeed result to default response.
    /// </summary>
    /// <param name="success">Some response</param>
    /// <param name="context">Http context for cookies managing or smth</param>
    /// <typeparam name="T">Some response type</typeparam>
    /// <returns>Related result</returns>
    public static IResult MapToResponse<T>(this T success, HttpContext? context = null)
    {
        return success switch
        {
            UserResponse s => s.MapUserResponse(context),
            bool s => Results.Ok(new BaseResponse()),
            _ => Results.Ok(new ResponseWithData<T>(success))
        };
    }
    
    /// <summary>
    /// Maps any EXPECTED (Not an actual!!!) exception to related result. If there is no some specific exception in this mapper, you should add it.
    /// </summary>
    /// <param name="error">Some EXPECTED error</param>
    /// <returns>Related result</returns>
    public static IResult MapToResponse(this Exception error)
    {
        (int statusCode, string message, IDictionary<string, object?>? additionalInfo) = error switch
        {
            UserWithTheSameEmailException e => (StatusCodes.Status409Conflict, e.Message, null),
            InvalidRefreshTokenException e => (StatusCodes.Status400BadRequest, e.Message, null),
            InvalidCredentialException e => (StatusCodes.Status400BadRequest, e.Message, null),
            DuplicateNameException e => (StatusCodes.Status409Conflict, e.Message, null),
            ValidationException e => e.ToValidationResponse(),
            _ => (StatusCodes.Status500InternalServerError, "An unmapable error occured.",
                new Dictionary<string, object?>())
        };

        return Results.Problem(
                statusCode: statusCode,
                title: message,
                extensions: additionalInfo
                );
    }

    private static (int statusCode, string message, IDictionary<string, object?>? additionalInfo) ToValidationResponse(this ValidationException error)
    {
        var info = new Dictionary<string, object?>
        {
            {"Validation Errors", error.Errors}
        };

        return (StatusCodes.Status400BadRequest, error.Message, info);
    }

    private static IResult MapUserResponse(this UserResponse success, HttpContext? context)
    {
        context.ThrowIfNull(new ExceptionCustomizations("Use MapToResponse with HttpContext"));
        
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true
            // probably set an expiration
        };
                        
        context.Response.Cookies
            .Append(Authentication.RefreshTokenCookieName, success.RefreshToken, cookieOptions);

        return Results.Ok(
            new ResponseWithData<UserResponseDto>(new(success))
            );
    }
}