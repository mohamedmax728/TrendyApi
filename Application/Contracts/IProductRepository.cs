using Application.Contracts.Persistence;
using Domain.Entities;

namespace Application.Contracts
{
    public interface IProductRepository : IBaseRepository<Product>
    {
    }
}
