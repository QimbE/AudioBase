using MediatR;

namespace Application.Labels.UpdateLabel;

public record UpdateLabelCommand(Guid Id, string Name, string Description, string PhotoLink): IRequest<Result<bool>>;