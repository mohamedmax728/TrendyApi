using Application.Common.Abstractions;
using Application.Common.Responses;
using Application.Contracts;
using Application.Features.Products.Dtos;
using AutoMapper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Application.Features.Products.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponse<CategoryDto>> CreateCategoryAsync(CreateUpdateCategoryDto createDto)
        {
            try
            {
                // Check if parent category exists if ParentId is provided
                if (createDto.ParentId.HasValue)
                {
                    var parentCategory = await _unitOfWork.ProductCategoryRepository.Value
                        .GetAsync(c => c.Id == createDto.ParentId.Value);
                    if (parentCategory == null)
                        return new BaseResponse<CategoryDto>
                        {
                            Success = false,
                            Message = "Parent category not found."
                        };
                }

                var category = _mapper.Map<ProductCategory>(createDto);
                await _unitOfWork.ProductCategoryRepository.Value.AddAsync(category);
                await _unitOfWork.SaveChangesAsync();

                var categoryDto = _mapper.Map<CategoryDto>(category);
                return new BaseResponse<CategoryDto>
                {
                    Success = true,
                    Message = "Category created successfully.",
                    Data = categoryDto
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<CategoryDto>
                {
                    Success = false,
                    Message = "An error occurred while creating the category.",
                    ValidationErrors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<BaseResponse<CategoryDto>> GetCategoryByIdAsync(int id)
        {
            var category = await _unitOfWork.ProductCategoryRepository.Value
                .GetAsync(
                    c => c.Id == id,
                    c => c.Parent);

            if (category == null)
                return new BaseResponse<CategoryDto>
                {
                    Success = false,
                    Message = "Category not found."
                };

            var categoryDto = _mapper.Map<CategoryDto>(category);
            return new BaseResponse<CategoryDto>
            {
                Success = true,
                Data = categoryDto
            };
        }

        public async Task<BaseResponse<IEnumerable<CategoryDto>>> GetAllCategoriesAsync()
        {
            var categories = await _unitOfWork.ProductCategoryRepository.Value
                .ListAsync(
                    includes: c => c.Parent);

            var categoryDtos = _mapper.Map<IEnumerable<CategoryDto>>(categories);
            return new BaseResponse<IEnumerable<CategoryDto>>
            {
                Success = true,
                Data = categoryDtos
            };
        }

        public async Task<BaseResponse<IEnumerable<CategoryDto>>> GetSubCategoriesAsync(int parentId)
        {
            var subCategories = await _unitOfWork.ProductCategoryRepository.Value
                .ListAsync(
                    filter: c => c.ParentId == parentId,
                    includes: c => c.Parent);

            var subCategoryDtos = _mapper.Map<IEnumerable<CategoryDto>>(subCategories);
            return new BaseResponse<IEnumerable<CategoryDto>>
            {
                Success = true,
                Data = subCategoryDtos
            };
        }

        public async Task<BaseResponse<CategoryDto>> UpdateCategoryAsync(int id, CreateUpdateCategoryDto updateDto)
        {
            try
            {
                var category = await _unitOfWork.ProductCategoryRepository.Value
                    .GetAsync(c => c.Id == id);
                if (category == null)
                    return new BaseResponse<CategoryDto>
                    {
                        Success = false,
                        Message = "Category not found."
                    };

                // Check if parent category exists if ParentId is provided
                if (updateDto.ParentId.HasValue)
                {
                    var parentCategory = await _unitOfWork.ProductCategoryRepository.Value
                        .GetAsync(c => c.Id == updateDto.ParentId.Value);
                    if (parentCategory == null)
                        return new BaseResponse<CategoryDto>
                        {
                            Success = false,
                            Message = "Parent category not found."
                        };

                    // Prevent setting itself as parent
                    if (updateDto.ParentId.Value == id)
                        return new BaseResponse<CategoryDto>
                        {
                            Success = false,
                            Message = "Category cannot be its own parent."
                        };
                }

                _mapper.Map(updateDto, category);
                await _unitOfWork.ProductCategoryRepository.Value.UpdateAsync(category);

                var categoryDto = _mapper.Map<CategoryDto>(category);
                return new BaseResponse<CategoryDto>
                {
                    Success = true,
                    Message = "Category updated successfully.",
                    Data = categoryDto
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<CategoryDto>
                {
                    Success = false,
                    Message = "An error occurred while updating the category.",
                    ValidationErrors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<BaseResponse> DeleteCategoryAsync(int id)
        {
            try
            {
                var category = await _unitOfWork.ProductCategoryRepository.Value
                    .GetAsync(c => c.Id == id);
                if (category == null)
                    return new BaseResponse
                    {
                        Success = false,
                        Message = "Category not found."
                    };

                // Check if category has subcategories
                var hasSubCategories = await _unitOfWork.ProductCategoryRepository.Value
                    .Exists(c => c.ParentId == id);
                if (hasSubCategories)
                    return new BaseResponse
                    {
                        Success = false,
                        Message = "Cannot delete category with existing subcategories."
                    };

                await _unitOfWork.ProductCategoryRepository.Value.DeleteAsync(category);
                await _unitOfWork.SaveChangesAsync();

                return new BaseResponse
                {
                    Success = true,
                    Message = "Category deleted successfully."
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse
                {
                    Success = false,
                    Message = "An error occurred while deleting the category.",
                    ValidationErrors = new List<string> { ex.Message }
                };
            }
        }
    }
}
