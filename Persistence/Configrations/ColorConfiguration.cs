using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configrations
{
    public class ColorConfiguration : IEntityTypeConfiguration<Color>
    {
        public void Configure(EntityTypeBuilder<Color> builder)
        {
            builder.HasKey(x => x.Id);
            
            builder.Property(c => c.NameEn)
                .IsRequired()
                .HasMaxLength(100);
                
            builder.Property(c => c.NameAr)
                .IsRequired()
                .HasMaxLength(100);
                
            builder.Property(c => c.ColorCode)
                .IsRequired()
                .HasMaxLength(20);
        }
    }
}
