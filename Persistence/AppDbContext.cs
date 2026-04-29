using Application.Common.Abstractions;
using Domain.Common;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Configrations;
using Persistence.Configurations;

namespace Persistence
{
    public class AppDbContext : DbContext
    {
        public ITenantProvider _tenantProvider;
        public AppDbContext()
        {

        }
        public AppDbContext(DbContextOptions<AppDbContext> options, ITenantProvider tenantProvider)
            : base(options)
        {
            _tenantProvider = tenantProvider;
        }
        public DbSet<Domain.Entities.User> Users { get; set; }
        public DbSet<Domain.Entities.Role> Roles { get; set; }
        public DbSet<Domain.Entities.PasswordResetToken> PasswordResetTokens { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    "data source=DESKTOP-5RA9U4V\\SQLEXPRESS;integrated security=SSPI;initial catalog=TrndyApiV2;trustservercertificate=True;MultipleActiveResultSets=True;"
                );
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            modelBuilder.ApplyConfiguration(new UserConfiguration(_tenantProvider));
            modelBuilder.ApplyConfiguration(new RoleConfiguration(_tenantProvider));
            modelBuilder.ApplyConfiguration(new PasswordResetTokenConfiguration());


            base.OnModelCreating(modelBuilder);
        }
       

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
        {
            foreach (var entry in ChangeTracker.Entries<AuditEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedDate = DateTime.UtcNow;
                        //entry.Entity.CreatedBy = ;
                        break;
                    case EntityState.Modified:
                        entry.Entity.ModifiedDate = DateTime.UtcNow;
                        //entry.Entity.ModifiedBy = ;
                        break;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
