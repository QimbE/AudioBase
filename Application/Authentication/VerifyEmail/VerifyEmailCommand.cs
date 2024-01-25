using MediatR;

namespace Application.Authentication.VerifyEmail;

public record VerifyEmailCommand(string Token): IRequest<Result<bool>>;