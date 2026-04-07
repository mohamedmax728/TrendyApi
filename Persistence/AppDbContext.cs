using Application.Common.Abstractions;
using Domain.Common;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

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
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    "Server=sqlserver,1433;Database=TaskManagementDb;User Id=sa;Password=TaskMana!1;TrustServerCertificate=True;"
                );
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            ConfigureUsers(modelBuilder);
            ConfigureRoles(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }
        private void ConfigureUsers(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(builder =>
            {
                builder.HasKey(x => x.Id);
                modelBuilder.Entity<Domain.Entities.User>()
                .HasIndex(u => u.Email)
                .IsUnique();
                builder.HasIndex(x => x.CompanyId);
                builder.HasQueryFilter(x => x.CompanyId == _tenantProvider.CompanyId);
            });
        }
        private void ConfigureRoles(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>(builder =>
            {
                builder.Property(u => u.RoleCode)
                       .HasConversion<string>();


            });
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
