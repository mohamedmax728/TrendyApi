using Application.Common.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configrations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        private readonly ITenantProvider _tenantProvider;

        public UserConfiguration(ITenantProvider tenantProvider)
        {
            _tenantProvider = tenantProvider;
        }

        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(u => u.Email).IsUnique();
            builder.HasIndex(x => x.CompanyId);
            // Removed default value from RoleId
            //builder.HasQueryFilter(x => x.CompanyId == _tenantProvider.CompanyId);
        }
    }
}
