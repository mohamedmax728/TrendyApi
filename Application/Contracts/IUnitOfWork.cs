using Application.Contracts.Persistence;

namespace Application.Contracts
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync();
        Lazy<IUserRepository> UserRepository { get; }
        Lazy<IRoleRepository> RoleRepository { get; }
    }
}
