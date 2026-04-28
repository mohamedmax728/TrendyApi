using Application.Features.Products.Dtos;
using Application.Common.Responses;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Products.Services
{
    public interface IProductService
    {
        Task<BaseResponse<ProductDto>> CreateProductAsync(CreateUpdateProductDto createProductDto);
        Task<BaseResponse<ProductDto>> GetProductByIdAsync(int id);
        Task<BaseResponse<IEnumerable<ProductDto>>> GetAllProductsAsync();
        Task<BaseResponse<ProductDto>> UpdateProductAsync(int id, CreateUpdateProductDto updateProductDto);
        Task<BaseResponse> DeleteProductAsync(int id);
    }
}
