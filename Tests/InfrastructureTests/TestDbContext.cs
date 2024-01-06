using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureTests;

/// <summary>
/// Context for tests
/// </summary>
public class TestDbContext : ApplicationDbContext
{
    public DbSet<EntityStub> Stubs { get; set; }
    
    public TestDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        
    }
}