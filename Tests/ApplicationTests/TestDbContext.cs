using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace ApplicationTests;

/// <summary>
/// Context for tests
/// </summary>
public class TestDbContext : ApplicationDbContext
{
    public TestDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {

    }
}