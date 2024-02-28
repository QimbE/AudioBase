﻿using Domain.Tracks;
using Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.DataAccess;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; set; }
    
    DbSet<Genre> Genres { get; }
    
    DbSet<Role> Roles { get; set; }
    
    DbSet<RefreshToken> RefreshTokens { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}