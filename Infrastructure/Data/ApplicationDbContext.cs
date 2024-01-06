using Application.DataAccess;
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