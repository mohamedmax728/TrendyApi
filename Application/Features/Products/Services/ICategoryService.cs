using Application.Common.Responses;
using Application.Features.Products.Dtos;

namespace Application.Features.Products.Services
{
    public interface ICategoryService
    {
        Task<BaseResponse<CategoryDto>> CreateCategoryAsync(CreateUpdateCategoryDto createDto);
        Task<BaseResponse<CategoryDto>> GetCategoryByIdAsync(int id);
        Task<BaseResponse<IEnumerable<CategoryDto>>> GetAllCategoriesAsync();
        Task<BaseResponse<IEnumerable<CategoryDto>>> GetSubCategoriesAsync(int parentId);
        Task<BaseResponse<CategoryDto>> UpdateCategoryAsync(int id, CreateUpdateCategoryDto updateDto);
        Task<BaseResponse> DeleteCategoryAsync(int id);
    }
}
