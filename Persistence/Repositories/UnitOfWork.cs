using Application.Contracts;
using Application.Contracts.Persistence;
using Application.Features.Authentication;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public UnitOfWork(AppDbContext context, IConfigurationProvider mapperConfig)
        {
            _context = context;
            UserRepository = new Lazy<IUserRepository>(() => new UserRepository(_context, mapperConfig));
            RoleRepository = new Lazy<IRoleRepository>(() => new RoleRepository(_context, mapperConfig));
            PasswordResetTokenRepository = new Lazy<IPasswordResetTokenRepository>(() => new PasswordResetTokenRepository(_context, mapperConfig));

        }
        public Lazy<IUserRepository> UserRepository { get; set; }
        public Lazy<IRoleRepository> RoleRepository { get; set; }
        public Lazy<IPasswordResetTokenRepository> PasswordResetTokenRepository { get; set; }


        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
