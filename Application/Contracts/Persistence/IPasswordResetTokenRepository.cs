using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Contracts.Persistence
{
    public interface IPasswordResetTokenRepository : IBaseRepository<Domain.Entities.PasswordResetToken>
    {
    }
}
