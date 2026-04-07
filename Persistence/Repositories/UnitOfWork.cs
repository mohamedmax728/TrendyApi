using Application.Contracts;
using Application.Contracts.Persistence;
using AutoMapper;

namespace Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context, IConfigurationProvider mapperConfig)
        {
            _context = context;
            UserRepository = new Lazy<IUserRepository>(() => new UserRepository(_context, mapperConfig));
        }

        public Lazy<IUserRepository> UserRepository { get; set; }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
