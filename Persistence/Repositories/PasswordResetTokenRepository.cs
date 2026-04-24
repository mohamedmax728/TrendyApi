using Application.Contracts.Persistence;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Persistence.Repositories
{
    public class PasswordResetTokenRepository : BaseRepository<Domain.Entities.PasswordResetToken>, IPasswordResetTokenRepository
    {
        public PasswordResetTokenRepository(DbContext context, IConfigurationProvider mapperConfig) : base(context, mapperConfig)
        {
        }
    }
}
