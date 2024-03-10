using MediatR;

namespace Application.Labels.CreateLabel;

public record CreateLabelCommand(string Name, string? Description, string PhotoLink): IRequest<Result<bool>>;