using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Genres.CreateGenre;

public record CreateGenreCommand (string Name): IRequest<Result<bool>>;