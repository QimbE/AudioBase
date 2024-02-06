using FluentValidation;

namespace Application.Authentication.RequestVerification;

public class RequestVerificationQueryValidator
    : AbstractValidator<RequestVerificationQuery>
{
    public RequestVerificationQueryValidator()
    {
        RuleFor(c => c.UserId).NotEmpty();
    }
}