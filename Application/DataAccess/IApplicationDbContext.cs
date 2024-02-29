using Domain.Artists;
using Domain.Junctions;
using Domain.Labels;
using Domain.MusicReleases;
using Domain.Tracks;
using Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.DataAccess;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; set; }
    
    DbSet<Role> Roles { get; set; }
    
    DbSet<RefreshToken> RefreshTokens { get; set; }
    
    DbSet<Track> Tracks { get; }
    
    DbSet<Genre> Genres { get; }
    
    DbSet<Release> Releases { get; }
    
    DbSet<ReleaseType> ReleaseTypes { get; }
    
    DbSet<Artist> Artists { get; }
    
    DbSet<CoAuthor> CoAuthors { get; }
    
    DbSet<Label> Labels { get; }
    
    DbSet<LabelRelease> LabelReleases { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}