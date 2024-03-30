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
        builder.HasKey(ca => ca.Id);

        builder.Property(ca => ca.TrackId).IsRequired();

        builder.Property(ca => ca.CoAuthorId).IsRequired();
        
        builder.HasOne<Artist>(ca=> ca.Artist)
            .WithMany()
            .HasForeignKey(ca => ca.CoAuthorId)
            .IsRequired();
        
        builder.HasOne<Track>(ca=>ca.Track)
            .WithMany(t=>t.CoAuthors)
            .HasForeignKey(ca => ca.TrackId)
            .IsRequired();
    }
}