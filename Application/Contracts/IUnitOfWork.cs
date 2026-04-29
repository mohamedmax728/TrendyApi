using Application.Contracts.Persistence;
using Application.Features.Authentication;

namespace Application.Contracts
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync();
        Lazy<IUserRepository> UserRepository { get; }
        Lazy<IRoleRepository> RoleRepository { get; }
        Lazy<IPasswordResetTokenRepository> PasswordResetTokenRepository { get; set; }
    }
}
