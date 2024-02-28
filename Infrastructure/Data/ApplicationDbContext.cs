using Application.DataAccess;
using Domain.Artists;
using Domain.Junctions;
using Domain.Labels;
using Domain.MusicReleases;
using Domain.Tracks;
using Domain.Users;
using Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

/// <summary>
/// Main AudioBase Db context
/// </summary>
public class ApplicationDbContext: DbContext, IApplicationDbContext
{
    /// <summary>
    /// Application users
    /// </summary>
    public DbSet<User> Users { get; set; }
    
    /// <summary>
    /// User roles
    /// </summary>
    public DbSet<Role> Roles { get; set; }
    
    /// <summary>
    /// User access refresh tokens
    /// </summary>
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    
    /// <summary>
    /// Integration and domain event list.
    /// </summary>
    public DbSet<OutboxMessage> OutboxMessages { get; set; }
    
    /// <summary>
    /// List of attempts to process outbox messages.
    /// </summary>
    public DbSet<OutboxMessageConsumer> OutboxMessageConsumers { get; set; }
    
    /// <summary>
    /// Music genres
    /// </summary>
    public DbSet<Genre> Genres { get; set; }
    
    /// <summary>
    /// Catalog tracks
    /// </summary>
    public DbSet<Track> Tracks { get; set; }
    
    /// <summary>
    /// Catalog artists
    /// </summary>
    public DbSet<Artist> Artists { get; set; }
    
    /// <summary>
    /// Catalog releases
    /// </summary>
    public DbSet<Release> Releases { get; set; }
    
    /// <summary>
    /// Catalog release types
    /// </summary>
    public DbSet<ReleaseType> ReleaseTypes { get; set; }
    
    // Not sure if next one is needed
    
    /// <summary>
    /// Catalog coauthors of tracks
    /// </summary>
    public DbSet<CoAuthor> CoAuthors { get; set; }
    
    /// <summary>
    /// Catalog labels
    /// </summary>
    public DbSet<Label> Labels { get; set; }
    
    // Not sure if next one is needed
    
    /// <summary>
    /// Catalog labels with their releases
    /// </summary>
    public DbSet<LabelRelease> LabelReleases { get; set; }
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        :base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}