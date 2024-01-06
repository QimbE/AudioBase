using System.Data;
using System.Security.Authentication;
using Domain.Users.Exceptions;
using LanguageExt.Common;
using Microsoft.AspNetCore.Http;

namespace Presentation;

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
        where T : class
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
    /// Maps any succeed result to default response.
    /// </summary>
    /// <param name="success">Some response</param>
    /// <typeparam name="T">Some response type</typeparam>
    /// <returns>Related result</returns>
    public static IResult MapToResponse<T>(this T success)
        where T: class
    {
        if (success is bool)
        {
            return Results.Ok(new BaseResponse());
        }

        return Results.Ok(new ResponseWithData(success));
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
            _ => (StatusCodes.Status500InternalServerError, "An unmapable error occured.",
                new Dictionary<string, object?>())
        };

        return Results.Problem(
                statusCode: statusCode,
                title: message,
                extensions: additionalInfo
                );
    }
}