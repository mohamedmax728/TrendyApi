using AutoMapper;
using Domain.Entities;

namespace Application.Features.Authentication.Dtos.Mapping
{
    public class AuthenticationMapping : Profile
    {
        public AuthenticationMapping()
        {
            CreateMap<RegisterRequestDto, User>()
           .ForMember(dest => dest.MobileNumber, opt => opt.MapFrom(src => src.PhoneNumber))
           .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
           .ForMember(dest => dest.PasswordSalt, opt => opt.Ignore())
           .ForMember(dest => dest.Role, opt => opt.Ignore())
           .ForMember(dest => dest.RoleId, opt => opt.Ignore());

        }
    }
}
