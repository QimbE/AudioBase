using Domain.Junctions;
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
    }
}