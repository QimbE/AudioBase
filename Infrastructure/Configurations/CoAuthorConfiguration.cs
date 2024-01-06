using Domain.Junctions;
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
    }
}