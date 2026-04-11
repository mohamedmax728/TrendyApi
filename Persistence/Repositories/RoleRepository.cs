using Application.Contracts.Persistence;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories
{
    public class RoleRepository : BaseRepository<Domain.Entities.Role>, IRoleRepository
    {
        public RoleRepository(DbContext context, IConfigurationProvider mapperConfig) : base(context, mapperConfig)
        {
        }
    }
}
