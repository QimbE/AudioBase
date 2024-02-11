using Domain.Labels;
using Domain.Tracks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class LabelConfiguration : IEntityTypeConfiguration<Label>
{
    public void Configure(EntityTypeBuilder<Label> builder)
    {
        builder.HasKey(l => l.Id);

        builder.HasIndex(l => l.Name).IsUnique();

        builder.Property(l => l.Name).IsRequired().HasMaxLength(50);
        
        // Description is nullable
        builder.Property(l => l.Description);
        
        builder.Property(l => l.PhotoLink).IsRequired();
    }
}