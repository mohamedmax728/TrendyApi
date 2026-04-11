using Application.Common.Responses;
using Application.Features.Authentication.Dtos;

namespace Application.Features.Authentication
{
    public interface IAuthenticationService
    {
        Task<BaseResponse> RegisterAsync(RegisterRequestDto request);
        Task<BaseResponse> LoginAsync(LoginRequestDto request);
    }
}
