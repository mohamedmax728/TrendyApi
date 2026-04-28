using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configrations
{
    public class PropertyConfiguration : IEntityTypeConfiguration<Property>
    {
        public void Configure(EntityTypeBuilder<Property> builder)
        {
            builder.HasKey(x => x.Id);
            
            builder.Property(p => p.Quantity)
                .IsRequired()
                .HasDefaultValue(0);
                
            builder.Property(p => p.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)");
                
            builder.Property(p => p.PriceAfterDiscount)
                .HasColumnType("decimal(18,2)");

            builder.HasOne(p => p.Product)
                .WithMany(p => p.Properties)
                .HasForeignKey(p => p.ProductId);
                
            builder.HasOne(p => p.Color)
                .WithMany()
                .HasForeignKey(p => p.ColorId);
                
            builder.HasOne(p => p.Size)
                .WithMany()
                .HasForeignKey(p => p.SizeId);
        }
    }
}
