using Application.Common.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        private readonly ITenantProvider _tenantProvider;

        public RoleConfiguration(ITenantProvider tenantProvider)
        {
            _tenantProvider = tenantProvider;
        }

        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.RoleCode).HasConversion<string>();
            builder.HasIndex(x => x.CompanyId);
            //builder.HasQueryFilter(x => x.CompanyId == _tenantProvider.CompanyId);
        }
    }
}