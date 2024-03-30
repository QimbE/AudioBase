using Domain.Favorites;
using Domain.Tracks;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class FavoriteConfiguration: IEntityTypeConfiguration<Favorite>
{
    public void Configure(EntityTypeBuilder<Favorite> builder)
    {
        builder.HasKey(f => f.Id);
        
        builder.Property(f => f.UserId).IsRequired();

        builder.Property(f => f.TrackId).IsRequired();
        
        builder.HasOne<User>(f=> f.User)
            .WithMany(u=>u.Favorites)
            .HasForeignKey(f => f.UserId)
            .IsRequired();
        
        builder.HasOne<Track>(f=>f.Track)
            .WithMany()
            .HasForeignKey(f => f.TrackId)
            .IsRequired();
    }
}