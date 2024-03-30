using Application.DataAccess;
using Domain.Artists;
using Domain.Labels;
using Domain.Tracks;
using Domain.Users;
using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Data;
using HotChocolate.Types;

namespace Application.GraphQL;

public class Endpoint
{
    [UsePaging(IncludeTotalCount = true)]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    [Authorize(nameof(Role.Admin))]
    public async Task<IQueryable<User>> GetUsers(
        [Service(ServiceKind.Resolver)] IApplicationDbContext context,
        CancellationToken cancellationToken
        )
    {
        return context.Users;
    }
    
    [UsePaging(IncludeTotalCount = true)]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    [Authorize(nameof(Role.DefaultUser))]
    public async Task<IQueryable<Genre>> GetGenres(
        [Service(ServiceKind.Resolver)] IApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        return context.Genres;
    }
    
    [UsePaging(IncludeTotalCount = true)]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    [Authorize(nameof(Role.DefaultUser))]
    public async Task<IQueryable<Artist>> GetArtists(
        [Service(ServiceKind.Resolver)] IApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        return context.Artists;
    }
    
    [UsePaging(IncludeTotalCount = true)]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    [Authorize(nameof(Role.DefaultUser))]
    public async Task<IQueryable<Label>> GetLabels(
        [Service(ServiceKind.Resolver)] IApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        return context.Labels;
    }
}