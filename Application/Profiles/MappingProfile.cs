using Application.Features.Authentication.Dtos;
using Application.Features.Authentication.Dtos.Mapping;
using Application.Features.Products.Dtos;
using AutoMapper;
using Domain.Entities;

namespace Application.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Authentication mappings
            CreateMap<RegisterRequestDto, User>();
            
            // Product mappings
            CreateMap<CreateUpdateProductDto, Product>();
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.Properties, opt => opt.MapFrom(src => src.Properties));

            // Category mappings
            CreateMap<CreateUpdateCategoryDto, ProductCategory>();
            CreateMap<ProductCategory, CategoryDto>();
        }
    }
}

