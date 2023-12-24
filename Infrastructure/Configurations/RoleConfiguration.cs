using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(r => r.Value).HasName("Id");

        builder.HasIndex(r => r.Name).IsUnique();

        builder.Property(r => r.Name).IsRequired().HasMaxLength(50);
        
        builder.HasData(Role.List);
    }
}