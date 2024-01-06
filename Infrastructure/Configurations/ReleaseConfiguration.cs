using Domain.Artists;
using Domain.MusicReleases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class ReleaseConfiguration : IEntityTypeConfiguration<Release>
{
    public void Configure(EntityTypeBuilder<Release> builder)
    {
        builder.HasKey(r => r.Id);
        
        builder.Property(r => r.Name).HasMaxLength(50);
        
        builder.Property(r => r.CoverLink);
        
        builder.Property(r => r.AuthorId);
        
        builder.Property(r => r.ReleaseTypeId);
        
        builder.Property(r => r.ReleaseDate);
        
        builder.HasOne<Artist>()
            .WithMany()
            .HasForeignKey(r => r.AuthorId)
            .IsRequired();
        
        builder.HasOne<ReleaseType>()
            .WithMany()
            .HasForeignKey(r => r.ReleaseTypeId)
            .IsRequired();
    }
}