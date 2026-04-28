using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configrations
{
    public class TrendMarkConfiguration : IEntityTypeConfiguration<TrendMark>
    {
        public void Configure(EntityTypeBuilder<TrendMark> builder)
        {
            builder.HasKey(x => x.Id);
            
            builder.Property(tm => tm.NameEn)
                .IsRequired()
                .HasMaxLength(200);
                
            builder.Property(tm => tm.NameAr)
                .IsRequired()
                .HasMaxLength(200);
                
            builder.Property(tm => tm.Image)
                .IsRequired()
                .HasColumnType("nvarchar(max)");
        }
    }
}
