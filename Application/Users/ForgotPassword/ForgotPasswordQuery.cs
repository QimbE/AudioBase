using MediatR;

namespace Application.Users.ForgotPassword;

public record ForgotPasswordQuery(string Email): IRequest<Result<bool>>;