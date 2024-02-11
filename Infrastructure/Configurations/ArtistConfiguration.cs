using Domain.Artists;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class ArtistConfiguration : IEntityTypeConfiguration<Artist>
{
    public void Configure(EntityTypeBuilder<Artist> builder)
    {
        builder.HasKey(a => a.Id);

        builder.HasIndex(a => a.Name).IsUnique();
        
        builder.Property(a => a.Name);
        
        // Description is nullable
        builder.Property(a => a.Description);
        
        builder.Property(a => a.PhotoLink).IsRequired();
    }
}