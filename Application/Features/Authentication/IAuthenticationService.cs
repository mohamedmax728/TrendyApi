using Application.Common.Responses;
using Application.Features.Authentication.Dtos;

namespace Application.Features.Authentication
{
    public interface IAuthenticationService
    {
        Task<BaseResponse> RegisterAsync(RegisterRequestDto request);
        Task<BaseResponse> LoginAsync(LoginRequestDto request);
        Task<BaseResponse> VerifyOtpAsync(VerifyOtpRequestDto request);
        Task<BaseResponse> ResetPasswordAsync(ResetPasswordRequestDto request);
        Task<BaseResponse> ForgotPasswordAsync(ForgotPasswordRequestDto request);



    }
}
