using FluentValidation;
using MediatR;
namespace Application.Behaviors;

/// <summary>
/// Validates request before give it to handler.
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public class ValidationPipelineBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, Result<TResponse>>
    where TRequest : IRequest<Result<TResponse>>
{
    private readonly IValidator<TRequest> _validator;
    
    public ValidationPipelineBehavior(IValidator<TRequest> validator)
    {
        _validator = validator;
    }
    
    public async Task<Result<TResponse>> Handle(TRequest request, RequestHandlerDelegate<Result<TResponse>> next, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return new(new ValidationException(validationResult.Errors));
        }

        // execute next part of pipeline
        return await next();
    }
}