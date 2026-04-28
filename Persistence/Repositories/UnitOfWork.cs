using Application.Contracts;
using Application.Contracts.Persistence;
using Application.Features.Authentication;
using AutoMapper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;

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
            ProductRepository = new Lazy<IProductRepository>(() => new ProductRepository(_context));
            ProductCategoryRepository = new Lazy<IBaseRepository<ProductCategory>>(() => new BaseRepository<ProductCategory>(_context, mapperConfig));
            TrendMarkRepository = new Lazy<IBaseRepository<TrendMark>>(() => new BaseRepository<TrendMark>(_context, mapperConfig));
            ColorRepository = new Lazy<IBaseRepository<Color>>(() => new BaseRepository<Color>(_context, mapperConfig));
            SizeRepository = new Lazy<IBaseRepository<Size>>(() => new BaseRepository<Size>(_context, mapperConfig));
            PropertyRepository = new Lazy<IBaseRepository<Property>>(() => new BaseRepository<Property>(_context, mapperConfig));
        }
        public Lazy<IUserRepository> UserRepository { get; set; }
        public Lazy<IRoleRepository> RoleRepository { get; set; }
        public Lazy<IPasswordResetTokenRepository> PasswordResetTokenRepository { get; set; }
        public Lazy<IProductRepository> ProductRepository { get; set; }
        public Lazy<IBaseRepository<ProductCategory>> ProductCategoryRepository { get; set; }
        public Lazy<IBaseRepository<TrendMark>> TrendMarkRepository { get; set; }
        public Lazy<IBaseRepository<Color>> ColorRepository { get; set; }
        public Lazy<IBaseRepository<Size>> SizeRepository { get; set; }
        public Lazy<IBaseRepository<Property>> PropertyRepository { get; set; }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}

