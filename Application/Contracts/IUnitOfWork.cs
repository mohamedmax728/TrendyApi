using Application.Contracts.Persistence;
using Application.Features.Authentication;
using Domain.Entities;

namespace Application.Contracts
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync();
        Lazy<IUserRepository> UserRepository { get; }
        Lazy<IRoleRepository> RoleRepository { get; }
        Lazy<IPasswordResetTokenRepository> PasswordResetTokenRepository { get; set; }
        Lazy<IProductRepository> ProductRepository { get; }
        Lazy<IBaseRepository<ProductCategory>> ProductCategoryRepository { get; }
        Lazy<IBaseRepository<TrendMark>> TrendMarkRepository { get; }
        Lazy<IBaseRepository<Color>> ColorRepository { get; }
        Lazy<IBaseRepository<Size>> SizeRepository { get; }
        Lazy<IBaseRepository<Property>> PropertyRepository { get; }
    }
}
