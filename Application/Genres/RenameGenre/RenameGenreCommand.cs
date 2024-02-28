using MediatR;

namespace Application.Genres.RenameGenre;

public record RenameGenreCommand(Guid Id, string NewName): IRequest<Result<bool>>;