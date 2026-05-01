using AutoMapper;
using Domain.Entities;

namespace Application.Features.Authentication.Dtos.Mapping
{
    public class AuthenticationMapping : Profile
    {
        public AuthenticationMapping()
        {
            CreateMap<RegisterRequestDto, User>()
                .ForMember(dest => dest.MobileNumber, opt => opt.MapFrom(src => src.PhoneNumber));

            CreateMap<User, LoginRequestDto>().ReverseMap();
        }
    }
}
