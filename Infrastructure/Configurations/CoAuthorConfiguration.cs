using Domain.Artists;
using Domain.Junctions;
using Domain.Tracks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class CoAuthorConfiguration: IEntityTypeConfiguration<CoAuthor>
{
    public void Configure(EntityTypeBuilder<CoAuthor> builder)
    {
        builder.HasKey(lr => lr.Id);

        builder.Property(lr => lr.TrackId).IsRequired();

        builder.Property(lr => lr.CoAuthorId).IsRequired();
        
        builder.HasOne<Artist>(ca=> ca.Artist)
            .WithMany()
            .HasForeignKey(lr => lr.CoAuthorId)
            .IsRequired();
        
        builder.HasOne<Track>(ca=>ca.Track)
            .WithMany(t=>t.CoAuthors)
            .HasForeignKey(lr => lr.TrackId)
            .IsRequired();
    }
}