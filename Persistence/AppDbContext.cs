using Application.Common.Abstractions;
using Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Persistence.Configrations;
using Persistence.Configurations;

namespace Persistence
{
    public class AppDbContext : DbContext
    {
        public ITenantProvider _tenantProvider;
        private readonly IConfiguration _configuration;

        public AppDbContext()
        {

        }
        public AppDbContext(DbContextOptions<AppDbContext> options, ITenantProvider tenantProvider,
            IConfiguration configuration)
            : base(options)
        {
            _tenantProvider = tenantProvider;
            _configuration = configuration;
        }
        public DbSet<Domain.Entities.User> Users { get; set; }
        public DbSet<Domain.Entities.Role> Roles { get; set; }
        public DbSet<Domain.Entities.PasswordResetToken> PasswordResetTokens { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Use connection string from configuration if available, otherwise use fallback
                var connectionString = _configuration?.GetConnectionString("ConnectionString");
                if (!string.IsNullOrEmpty(connectionString))
                {
                    optionsBuilder.UseSqlServer(connectionString);
                }
                else
                {
                    // Fallback connection string for development
                    optionsBuilder.UseSqlServer(
                        "Server=sqlserver,1433;Database=TaskManagementDb;User Id=sa;Password=TaskMana!1;TrustServerCertificate=True;"
                    );
                }
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
