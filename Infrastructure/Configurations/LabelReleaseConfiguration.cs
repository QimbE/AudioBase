using Domain.Junctions;
using Domain.Labels;
using Domain.MusicReleases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class LabelReleaseConfiguration: IEntityTypeConfiguration<LabelRelease>
{
    public void Configure(EntityTypeBuilder<LabelRelease> builder)
    {
        builder.HasKey(lr => lr.Id);

        builder.Property(lr => lr.LabelId).IsRequired();

        builder.Property(lr => lr.ReleaseId).IsRequired();
        
        builder.HasOne<Label>(lr=>lr.Label)
            .WithMany(l=>l.LabelReleases)
            .HasForeignKey(lr => lr.LabelId)
            .IsRequired();
        
        builder.HasOne<Release>(lr=>lr.Release)
            .WithMany(r=>r.LabelReleases)
            .HasForeignKey(lr => lr.ReleaseId)
            .IsRequired();
    }
}