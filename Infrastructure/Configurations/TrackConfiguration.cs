using Domain.Tracks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class TrackConfiguration: IEntityTypeConfiguration<Track>
{
    public void Configure(EntityTypeBuilder<Track> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name).IsRequired().HasMaxLength(100);
        
        builder.Property(t => t.AudioLink).IsRequired();
        
        builder.Property(t => t.Duration).IsRequired();
        
        builder.Property(t => t.ReleaseId).IsRequired();
        
        builder.Property(t => t.GenreId).IsRequired();

        builder.HasOne<Genre>()
            .WithMany()
            .HasForeignKey(o => o.GenreId)
            .IsRequired();
    }
}