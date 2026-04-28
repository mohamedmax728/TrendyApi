using Application.Common.Abstractions;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configrations
{
    public class RoleConfigration : IEntityTypeConfiguration<Role>
    {
        private readonly ITenantProvider _tenantProvider;

        public RoleConfigration(ITenantProvider tenantProvider)
        {
            _tenantProvider = tenantProvider;
        }

        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.RoleCode).HasConversion<string>();
            builder.HasIndex(x => x.CompanyId);

            // Seed initial roles
            builder.HasData(
                new Role { Id = 1, Name = "Admin", RoleCode = RoleCodeEnum.Admin, CompanyId = 0 },
                new Role { Id = 2, Name = "Vendor", RoleCode = RoleCodeEnum.Vendor, CompanyId = 0 },
                new Role { Id = 3, Name = "Customer", RoleCode = RoleCodeEnum.Customer, CompanyId = 0 }
            );

            //builder.HasQueryFilter(x => x.CompanyId == _tenantProvider.CompanyId);
        }
    }
}
