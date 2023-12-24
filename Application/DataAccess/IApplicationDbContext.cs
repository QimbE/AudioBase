using Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.DataAccess;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; set; }
    
    DbSet<Role> Roles { get; set; }
    
    DbSet<RefreshToken> RefreshTokens { get; set; }
}