using Application.DataAccess;
using Domain.Artists;
using Domain.Favorites;
using Domain.Labels;
using Domain.MusicReleases;
using Domain.Tracks;
using Domain.Users;
using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Data;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore.Query.Internal;

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
    
    // Gets junction table data
    [UsePaging(IncludeTotalCount = true)]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    [Authorize(nameof(Role.DefaultUser))]
    public async Task<IQueryable<Favorite>> GetFavorites(
        [Service(ServiceKind.Resolver)] IApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        return context.Favorites;
    }
    
    // Gets Users favorite tracks
    [UsePaging(IncludeTotalCount = true)]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    [Authorize(nameof(Role.DefaultUser))]
    public async Task<IQueryable<Track>> GetFavoriteTracks(
        [Service(ServiceKind.Resolver)] IApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        List<Track> tracks = new List<Track>();
        foreach (var fav in context.Favorites)
        {
            tracks.Append(fav.Track);
        }
        return new EnumerableQuery<Track>(tracks);
    }
    
    [UsePaging(IncludeTotalCount = true)]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    [Authorize(nameof(Role.DefaultUser))]
    public async Task<IQueryable<Release>> GetReleases(
        [Service(ServiceKind.Resolver)] IApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        return context.Releases;
    }
    
    [UsePaging(IncludeTotalCount = true)]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    [Authorize(nameof(Role.DefaultUser))]
    public async Task<IQueryable<Track>> GetTracks(
        [Service(ServiceKind.Resolver)] IApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        return context.Tracks;
    }
}