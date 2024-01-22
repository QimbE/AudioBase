using MediatR;

namespace Application.Users.ChangePassword;

public record ChangePasswordCommand(Guid UserId, string OldPassword, string NewPassword): IRequest<Result<bool>>;