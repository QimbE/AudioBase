using MediatR;

namespace Application.Authentication.Register;

public record RegisterCommand(string Name, string Email, string Password): IRequest<Result<bool>>;