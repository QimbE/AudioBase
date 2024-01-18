using MediatR;

namespace Application.Users.ChangeRole;

public record ChangeRoleCommand(Guid UserId, string RoleName): IRequest<Result<bool>>;