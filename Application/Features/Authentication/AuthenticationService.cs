using Application.Common.Responses;
using Application.Contracts;
using Application.Features.Authentication.Dtos;

namespace Application.Features.Authentication
{
    public class AuthenticationService(IUnitOfWork _unitOfWork) : IAuthenticationService
    {
        public async Task<BaseResponse> RegisterAsync(RegisterRequestDto request)
        {
            try
            {
                var checkIfEmailOrPhoneExists = await CheckIfEmailOrMobileNumberExists(request.Email, request.PhoneNumber);
                if (checkIfEmailOrPhoneExists)
                {
                    return new BaseResponse
                    {
                        Success = false,
                        Message = "Email or phone number already exists.",
                        ValidationErrors = new List<string> { "Email or phone number already exists." }
                    };
                }

                var x = await _unitOfWork.UserRepository.Value.GetAsync(
                    s => s.Email == request.Email
                    , s => s.Role);

                return new BaseResponse
                {
                    Success = true,
                    Message = "Email and phone number are available."
                };
            }
            catch (Exception ex)
            {
                throw new NotImplementedException($"delete this line when you implement this method," +
                    $" this is just to avoid unhandled exception in case of any error in the method," +
                    $" you can also log the exception in database or serilog if you want to. {ex.Message}");
                return new BaseResponse
                {
                    Success = false,
                    Message = "An error occurred while checking email or phone number.",
                    ValidationErrors = new List<string> { ex.Message }
                };
            }
        }

        public Task<BaseResponse> LoginAsync(LoginRequestDto request)
        {
            throw new NotImplementedException();
        }
        private async Task<bool> CheckIfEmailOrMobileNumberExists(string email, string mobileNumber)
        {
            return await _unitOfWork.UserRepository.Value
                .Exists(s => s.Email.ToLower() == email.ToLower() || s.MobileNumber.ToLower() == mobileNumber.ToLower());
        }
    }
}
