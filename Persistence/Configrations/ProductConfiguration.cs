using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configrations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(x => x.Id);
            
            builder.Property(p => p.NameEn)
                .IsRequired()
                .HasMaxLength(200);
                
            builder.Property(p => p.NameAr)
                .IsRequired()
                .HasMaxLength(200);
                
                
            builder.Property(p => p.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)");
                
            builder.Property(p => p.PriceAfterDiscount)
                .HasColumnType("decimal(18,2)");
                
            builder.Property(p => p.Image)
                .HasColumnType("nvarchar(max)");
                
            builder.Property(p => p.Quantity)
                .IsRequired()
                .HasDefaultValue(0);
                
                
            builder.Property(p => p.YouTubeLink)
                .HasColumnType("nvarchar(max)");

            builder.HasOne(p => p.Category)
                .WithMany()
                .HasForeignKey(p => p.CategoryId);
                
            builder.HasOne(p => p.SubCategory)
                .WithMany()
                .HasForeignKey(p => p.SubCategoryId);

            builder.HasOne(p => p.TrendMark)
                .WithMany()
                .HasForeignKey(p => p.TrendMarkId);

            builder.HasOne(p => p.Vendor)
                .WithMany()
                .HasForeignKey(p => p.VendorId);
        }
    }
}
