using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configrations
{
    public class ProductCategoryConfiguration : IEntityTypeConfiguration<ProductCategory>
    {
        public void Configure(EntityTypeBuilder<ProductCategory> builder)
        {
            builder.HasKey(x => x.Id);
            
            builder.Property(p => p.NameEn)
                .HasMaxLength(200);
                
            builder.Property(p => p.NameAr)
                .HasMaxLength(200);
                
            builder.Property(p => p.Description)
                .HasColumnType("nvarchar(max)");
            
            // Self-referencing relationship for parent/child categories
            builder.HasOne(p => p.Parent)
                .WithMany()
                .HasForeignKey(p => p.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
