using Domain.Images;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

public class ImageConfiguration : IEntityTypeConfiguration<Image>
{
    public void Configure(EntityTypeBuilder<Image> builder) 
    {
        builder.ToTable("Images");
        
        builder.HasKey(i => i.Id);    
        builder.Property(i => i.Id)
            .ValueGeneratedOnAdd()
            .IsRequired();
    
        builder.Property(i => i.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(i => i.Description)
            .HasMaxLength(500);

        builder.Property(i => i.Path)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(i => i.Height)
            .HasDefaultValue(0);

        builder.Property(i => i.Width)
            .HasDefaultValue(0);

        builder.Property(i => i.Size)
            .HasDefaultValue(0);

        builder.HasOne(i => i.User)
            .WithMany(u => u.Images)
            .HasForeignKey(i => i.UserId);

    }
}
