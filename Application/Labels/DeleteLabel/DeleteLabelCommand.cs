using MediatR;

namespace Application.Labels.DeleteLabel;

public record DeleteLabelCommand (Guid Id): IRequest<Result<bool>>;