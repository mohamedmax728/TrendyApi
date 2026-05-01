using Application.Common.Abstractions;
using Application.Common.Helpers;
using Application.Common.Responses;
using Application.Contracts;
using Application.Features.Authentication.Dtos;
using AutoMapper;
using Domain.Entities;

namespace Application.Features.Authentication
{
    public class AuthenticationService(IUnitOfWork _unitOfWork, IMapper _mapper, IJwtTokenGenerator jwtTokenGenerator, IEmailService _emailService) : IAuthenticationService
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

                // Get Customer role by RoleCode instead of hardcoded ID
                var customerRole = await _unitOfWork.RoleRepository.Value
                    .GetAsync(x => x.RoleCode == Domain.Enums.RoleCodeEnum.Customer);

                if (customerRole == null)
                {
                    return new BaseResponse
                    {
                        Success = false,
                        Message = "Customer role not found.",
                        ValidationErrors = new List<string> { "Customer role not configured in database." }
                    };
                }

                PasswordMaker.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

                var user = _mapper.Map<User>(request);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                user.RoleId = customerRole.Id;

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
            try
            {
                var user = await _unitOfWork.UserRepository.Value.GetAsync(x => x.Email == request.Email, x => x.Role);
                if (user == null)
                    return new BaseResponse("Email or password is incorrect.", false);

                var isValid = PasswordMaker.VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt);
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
            catch (Exception ex)
            {
                return new BaseResponse
                {
                    Success = false,
                    Message = "An error occurred while logging in.",
                    ValidationErrors = new List<string> { ex.Message }
                };
            }
        }
        private async Task<bool> CheckIfEmailOrMobileNumberExists(string email, string mobileNumber)
        {
            var emailLower = email.ToLower();
            var mobileLower = mobileNumber.ToLower();

            return await _unitOfWork.UserRepository.Value
                .Exists(s => s.Email.ToLower() == emailLower
                          || s.MobileNumber.ToLower() == mobileLower);
        }

        public async Task<BaseResponse> ResetPasswordAsync(ResetPasswordRequestDto request)
        {
            try
            {
                if (request.NewPassword != request.ConfirmPassword)
                    return new BaseResponse("Passwords do not match.", false);

                var resetToken = await _unitOfWork.PasswordResetTokenRepository.Value
                    .GetAsync(x => x.User.Email == request.Email
                                && x.OTP == request.OTP
                                && !x.IsUsed
                                && x.ExpiresAt > DateTime.UtcNow);

                if (resetToken == null)
                    return new BaseResponse("Invalid or expired OTP.", false);

                var user = await _unitOfWork.UserRepository.Value
                    .GetAsync(x => x.Email == request.Email);

                PasswordMaker.CreatePasswordHash(request.NewPassword, out byte[] hash, out byte[] salt);
                user.PasswordHash = hash;
                user.PasswordSalt = salt;

                await _unitOfWork.UserRepository.Value.UpdateAsync(user);

                resetToken.IsUsed = true;
                await _unitOfWork.PasswordResetTokenRepository.Value.UpdateAsync(resetToken);

                return new BaseResponse("Password reset successfully.");
            }
            catch (Exception ex)
            {
                return new BaseResponse
                {
                    Success = false,
                    Message = "An error occurred while resetting password.",
                    ValidationErrors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<BaseResponse> VerifyOtpAsync(VerifyOtpRequestDto request)
        {
            try
            {
                var resetToken = await _unitOfWork.PasswordResetTokenRepository.Value
                    .GetAsync(x => x.User.Email == request.Email
                                && x.OTP == request.OTP
                                && !x.IsUsed
                                && x.ExpiresAt > DateTime.UtcNow);

                if (resetToken == null)
                    return new BaseResponse("Invalid or expired OTP.", false);

                return new BaseResponse("OTP is valid.");
            }
            catch (Exception ex)
            {
                return new BaseResponse
                {
                    Success = false,
                    Message = "An error occurred while verifying OTP.",
                    ValidationErrors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<BaseResponse> ForgotPasswordAsync(ForgotPasswordRequestDto request)
        {
            try
            {
                var user = await _unitOfWork.UserRepository.Value
                    .GetAsync(x => x.Email == request.Email);

                if (user == null)
                    return new BaseResponse("Email not found.", false);

                var otp = new Random().Next(100000, 999999).ToString();

                var resetToken = new PasswordResetToken
                {
                    UserId = user.Id,
                    OTP = otp,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(10),
                    IsUsed = false,
                    CompanyId = user.CompanyId
                };

                await _unitOfWork.PasswordResetTokenRepository.Value.AddAsync(resetToken);

                await _emailService.SendAsync(user.Email, "Password Reset OTP", $"Your OTP is: {otp}");

                return new BaseResponse("OTP sent to your email.");
            }
            catch (Exception ex)
            {
                return new BaseResponse
                {
                    Success = false,
                    Message = "An error occurred while processing forgot password.",
                    ValidationErrors = new List<string> { ex.Message }
                };
            }
        }
    }
}
