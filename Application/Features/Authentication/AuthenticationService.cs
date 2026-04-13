using Application.Common.Abstractions;
using Application.Common.Helpers;
using Application.Common.Responses;
using Application.Contracts;
using Application.Features.Authentication.Dtos;
using Domain.Entities;
using AutoMapper;

namespace Application.Features.Authentication
{
    public class AuthenticationService(IUnitOfWork _unitOfWork , IMapper _mapper , IJwtTokenGenerator jwtTokenGenerator) : IAuthenticationService
    {
        public async Task<BaseResponse> RegisterAsync(RegisterRequestDto request)
        {
            try
            {
                var exists = await CheckIfEmailOrMobileNumberExists(request.Email, request.PhoneNumber);
                if (exists)
                {
                    return new BaseResponse
                    {
                        Success = false,
                        Message = "Email or phone number already exists.",
                        ValidationErrors = new List<string> { "Email or phone number already exists." }
                    };
                }

                PasswordMaker.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

                var user = _mapper.Map<User>(request);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;

                await _unitOfWork.UserRepository.Value.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();

                return new BaseResponse
                {
                    Success = true,
                    Message = "User registered successfully."
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse
                {
                    Success = false,
                    Message = "An error occurred while registering.",
                    ValidationErrors = new List<string> { ex.Message }
                };
            }
        }
        public async Task<BaseResponse> LoginAsync(LoginRequestDto request)
        {
            var user = await _unitOfWork.UserRepository.Value.GetAsync(x => x.Email == request.Email,x=>x.Role);
            if (user == null)
                 return new BaseResponse("Email or password is incorrect.", false);
       
             var isValid = PasswordMaker.VerifyPassword(request.Password,user.PasswordHash,user.PasswordSalt);     
             if (!isValid)
                 return new BaseResponse("Email or password is incorrect.", false);         
             var token = jwtTokenGenerator.GenerateToken(user);
             return new BaseResponse<string>
             {
                 Success = true,
                 Message = "Login successful.",
                 Data = token
             };

        }
        private async Task<bool> CheckIfEmailOrMobileNumberExists(string email, string mobileNumber)
        {
            var emailLower = email.ToLower();
            var mobileLower = mobileNumber.ToLower();

            return await _unitOfWork.UserRepository.Value
                .Exists(s => s.Email.ToLower() == emailLower
                          || s.MobileNumber.ToLower() == mobileLower);
        }
    }
}
