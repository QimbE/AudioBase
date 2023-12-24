using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class UserConfiguration: IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.Email).IsRequired();

        builder.Property(u => u.Name).IsRequired().HasMaxLength(100);

        builder.Property(u => u.Password).IsRequired();

        builder.Property(u => u.RoleId).IsRequired();

        // One-to-one with refresh token
        builder
            .HasOne(u => u.RefreshToken)
            .WithOne(r => r.Owner)
            .HasForeignKey<RefreshToken>(r => r.Id);

        // Many-to-one with role
        builder
            .HasOne(u => u.Role)
            .WithMany()
            .HasForeignKey(u => u.RoleId);
    }
}