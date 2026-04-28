using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configrations
{
    public class SizeConfiguration : IEntityTypeConfiguration<Size>
    {
        public void Configure(EntityTypeBuilder<Size> builder)
        {
            builder.HasKey(x => x.Id);
            
            builder.Property(s => s.NameEn)
                .IsRequired()
                .HasMaxLength(100);
                
            builder.Property(s => s.NameAr)
                .IsRequired()
                .HasMaxLength(100);
        }
    }
}
