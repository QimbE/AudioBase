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
        
        builder.Property(r => r.Name).IsRequired().HasMaxLength(50);
        
        builder.Property(r => r.CoverLink).IsRequired();
        
        builder.Property(r => r.AuthorId).IsRequired();
        
        builder.Property(r => r.ReleaseTypeId).IsRequired();
        
        builder.Property(r => r.ReleaseDate).IsRequired();
        
        builder.HasOne<Artist>()
            .WithMany()
            .HasForeignKey(r => r.AuthorId)
            .IsRequired();
        
        builder.HasOne(r => r.ReleaseType)
            .WithMany()
            .HasForeignKey(r => r.ReleaseTypeId);
    }
}