using Application.Common.Abstractions;
using Application.Common.Helpers;
using Application.Common.Responses;
using Application.Contracts;
using Application.Features.Products.Dtos;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Text.Json;
using System.Linq;

namespace Application.Features.Products.Services
{
    public class ProductService(
        IUnitOfWork _unitOfWork,
        IProductRepository _productRepository,
        IMapper _mapper) : IProductService
    {
        public async Task<BaseResponse<ProductDto>> CreateProductAsync(CreateUpdateProductDto createProductDto)
        {
            try
            {
                // Validate category if provided
             
                
                var category = await _unitOfWork.ProductCategoryRepository.Value
                    .GetAsync(c => c.Id == createProductDto.CategoryId);
                if (category == null)
                    return new BaseResponse<ProductDto>
                    {
                        Success = false,
                        Message = "Category not found."
                    };
             

                // Validate sub-category if provided
                if (createProductDto.SubCategoryId.HasValue)
                {
                    var subCategory = await _unitOfWork.ProductCategoryRepository.Value
                        .GetAsync(c => c.Id == createProductDto.SubCategoryId.Value);
                    if (subCategory == null)
                        return new BaseResponse<ProductDto>
                        {
                            Success = false,
                            Message = "Sub-category not found."
                        };
                    
                    // Verify sub-category has a parent
                    if (!subCategory.ParentId.HasValue && subCategory.ParentId.Value != createProductDto.SubCategoryId.Value)
                        return new BaseResponse<ProductDto>
                        {
                            Success = false,
                            Message = "Selected sub-category does not Match a  category."
                        };
                }

                // Validate trend mark if provided
                if (createProductDto.TrendMarkId.HasValue)
                {
                    var trendMark = await _unitOfWork.TrendMarkRepository.Value
                        .GetAsync(tm => tm.Id == createProductDto.TrendMarkId.Value);
                    if (trendMark == null)
                        return new BaseResponse<ProductDto>
                        {
                            Success = false,
                            Message = "TrendMark not found."
                        };
                }

                 var vendor = await _unitOfWork.UserRepository.Value
                     .GetAsync(v => v.Id == createProductDto.VendorId);
                 if (vendor == null)
                     return new BaseResponse<ProductDto>
                     {
                         Success = false,
                         Message = "Vendor (User) not found."
                     };
                

                // Map to product entity
                var product = _mapper.Map<Product>(createProductDto);

                // Create properties if provided
                if (createProductDto.Properties != null && createProductDto.Properties.Any())
                {
                    foreach (var propDto in createProductDto.Properties)
                    {
                        var property = new Property
                        {
                            ProductId = product.Id,
                            ColorId = propDto.ColorId,
                            SizeId = propDto.SizeId,
                            Quantity = propDto.Quantity,
                            Price = propDto.Price,
                            PriceAfterDiscount = propDto.PriceAfterDiscount
                        };
                        product.Properties.Add(property);
                    }
                }

                // Save to database
                await _productRepository.AddAsync(product);
                await _unitOfWork.SaveChangesAsync();

                // Map to DTO for response
                var productDto = _mapper.Map<ProductDto>(product);

                return new BaseResponse<ProductDto>
                {
                    Success = true,
                    Message = "Product created successfully.",
                    Data = productDto
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<ProductDto>
                {
                    Success = false,
                    Message = "An error occurred while creating the product.",
                    ValidationErrors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<BaseResponse<ProductDto>> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetAsync(p => p.Id == id);
            if (product == null)
                return new BaseResponse<ProductDto>
                {
                    Success = false,
                    Message = "Product not found."
                };

            var productDto = _mapper.Map<ProductDto>(product);

            return new BaseResponse<ProductDto>
            {
                Success = true,
                Data = productDto
            };
        }

        public async Task<BaseResponse<IEnumerable<ProductDto>>> GetAllProductsAsync()
        {
            var products = await _productRepository.ListAsync();
            var productDtos = _mapper.Map<List<ProductDto>>(products);

            return new BaseResponse<IEnumerable<ProductDto>>
            {
                Success = true,
                Data = productDtos
            };
        }

        public async Task<BaseResponse<ProductDto>> UpdateProductAsync(int id, CreateUpdateProductDto updateProductDto)
        {
            try
            {
                var product = await _productRepository.GetAsync(p => p.Id == id);
                if (product == null)
                    return new BaseResponse<ProductDto>
                    {
                        Success = false,
                        Message = "Product not found."
                    };

                // Validate category if provided
                
                var category = await _unitOfWork.ProductCategoryRepository.Value
                    .GetAsync(c => c.Id == updateProductDto.CategoryId);
                if (category == null)
                    return new BaseResponse<ProductDto>
                    {
                        Success = false,
                        Message = "Category not found."
                    };


                if (updateProductDto.SubCategoryId.HasValue)
                {
                    var subCategory = await _unitOfWork.ProductCategoryRepository.Value
                        .GetAsync(c => c.Id == updateProductDto.SubCategoryId.Value);
                    if (subCategory == null)
                        return new BaseResponse<ProductDto>
                        {
                            Success = false,
                            Message = "Sub-category not found."
                        };

                    // Verify sub-category has a parent
                    if (!subCategory.ParentId.HasValue && subCategory.ParentId.Value != updateProductDto.SubCategoryId.Value)
                        return new BaseResponse<ProductDto>
                        {
                            Success = false,
                            Message = "Selected sub-category does not Match a  category."
                        };
                }

                // Validate trend mark if provided
                if (updateProductDto.TrendMarkId.HasValue)
                {
                    var trendMark = await _unitOfWork.TrendMarkRepository.Value
                        .GetAsync(tm => tm.Id == updateProductDto.TrendMarkId.Value);
                    if (trendMark == null)
                        return new BaseResponse<ProductDto>
                        {
                            Success = false,
                            Message = "TrendMark not found."
                        };
                }

                // Validate vendor/user if provided
                
                    var vendor = await _unitOfWork.UserRepository.Value
                        .GetAsync(v => v.Id == updateProductDto.VendorId);
                    if (vendor == null)
                        return new BaseResponse<ProductDto>
                        {
                            Success = false,
                            Message = "Vendor (User) not found."
                        };
                

                // Update existing product properties - clear and re-add
                var existingProperties = await _unitOfWork.PropertyRepository.Value
                    .ListAsync(p => p.ProductId == id);
                foreach (var prop in existingProperties)
                {
                    await _unitOfWork.PropertyRepository.Value.DeleteAsync(prop);
                }

                // Add new properties if provided
                if (updateProductDto.Properties != null && updateProductDto.Properties.Any())
                {
                    foreach (var propDto in updateProductDto.Properties)
                    {
                        var property = new Property
                        {
                            ProductId = product.Id,
                            ColorId = propDto.ColorId,
                            SizeId = propDto.SizeId,
                            Quantity = propDto.Quantity,
                            Price = propDto.Price,
                            PriceAfterDiscount = propDto.PriceAfterDiscount
                        };
                        product.Properties.Add(property);
                    }
                }

                // Update product entity
                _mapper.Map(updateProductDto, product);

                await _productRepository.UpdateAsync(product);
                await _unitOfWork.SaveChangesAsync();

                // Map to DTO for response
                var productDto = _mapper.Map<ProductDto>(product);

                return new BaseResponse<ProductDto>
                {
                    Success = true,
                    Message = "Product updated successfully.",
                    Data = productDto
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<ProductDto>
                {
                    Success = false,
                    Message = "An error occurred while updating the product.",
                    ValidationErrors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<BaseResponse> DeleteProductAsync(int id)
        {
            var product = await _productRepository.GetAsync(p => p.Id == id);
            if (product == null)
                return new BaseResponse
                {
                    Success = false,
                    Message = "Product not found."
                };

            await _productRepository.DeleteAsync(product);
            await _unitOfWork.SaveChangesAsync();

            return new BaseResponse
            {
                Success = true,
                Message = "Product deleted successfully."
            };
        }
    }
}
