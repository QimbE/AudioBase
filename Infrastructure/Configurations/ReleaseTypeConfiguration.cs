using Domain.MusicReleases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class ReleaseTypeConfiguration : IEntityTypeConfiguration<ReleaseType>
{
    public void Configure(EntityTypeBuilder<ReleaseType> builder)
    {
        // Release type is SmartEmum, therefore definition is different
        builder.HasKey(rt => rt.Value);

        builder.Property(rt => rt.Value).HasColumnName("Id");

        builder.HasIndex(rt => rt.Name).IsUnique();

        builder.Property(rt => rt.Name).IsRequired().HasMaxLength(50);
        
        // Returns whole list of release types
        builder.HasData(ReleaseType.List);
    }
}