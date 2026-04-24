using Application.Common.Abstractions;
using Application.Common.Helpers;
using Application.Common.Responses;
using Application.Contracts;
using Application.Features.Authentication.Dtos;
using Domain.Entities;
using AutoMapper;

namespace Application.Features.Authentication
{
    public class AuthenticationService(IUnitOfWork _unitOfWork , IMapper _mapper , IJwtTokenGenerator jwtTokenGenerator ,IEmailService _emailService) : IAuthenticationService
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

        public async Task<BaseResponse> ResetPasswordAsync(ResetPasswordRequestDto request)
        {
            if (request.NewPassword != request.ConfirmPassword)
                return new BaseResponse("Passwords do not match.", false);

            // 1. تتحقق من الـ OTP
            var resetToken = await _unitOfWork.PasswordResetTokenRepository.Value
                .GetAsync(x => x.User.Email == request.Email
                            && x.OTP == request.OTP
                            && !x.IsUsed
                            && x.ExpiresAt > DateTime.UtcNow);

            if (resetToken == null)
                return new BaseResponse("Invalid or expired OTP.", false);

            // 2. جيب الـ User
            var user = await _unitOfWork.UserRepository.Value
                .GetAsync(x => x.Email == request.Email);

            // 3. عمل Hash للـ Password الجديد
            PasswordMaker.CreatePasswordHash(request.NewPassword, out byte[] hash, out byte[] salt);
            user.PasswordHash = hash;
            user.PasswordSalt = salt;

            // 4. حدث الـ User
            await _unitOfWork.UserRepository.Value.UpdateAsync(user);

            // 5. اعمل الـ OTP used
            resetToken.IsUsed = true;
            await _unitOfWork.PasswordResetTokenRepository.Value.UpdateAsync(resetToken);

            return new BaseResponse("Password reset successfully.");
        }

        public async Task<BaseResponse> VerifyOtpAsync(VerifyOtpRequestDto request)
        {
            // 1. جيب الـ OTP من الداتابيز
            var resetToken = await _unitOfWork.PasswordResetTokenRepository.Value
                .GetAsync(x => x.User.Email == request.Email
                            && x.OTP == request.OTP
                            && !x.IsUsed
                            && x.ExpiresAt > DateTime.UtcNow);

            if (resetToken == null)
                return new BaseResponse("Invalid or expired OTP.", false);

            return new BaseResponse("OTP is valid.");
        }

        public async Task<BaseResponse> ForgotPasswordAsync(ForgotPasswordRequestDto request)
        {
            // 1. تتحقق إن الـ Email موجود
            var user = await _unitOfWork.UserRepository.Value
                .GetAsync(x => x.Email == request.Email);

            if (user == null)
                return new BaseResponse("Email not found.", false);

            // 2. تعمل OTP
            var otp = new Random().Next(100000, 999999).ToString();

            // 3. تحفظه في الداتابيز
            var resetToken = new PasswordResetToken
            {
                UserId = user.Id,
                OTP = otp,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10),
                IsUsed = false,
                CompanyId = user.CompanyId
            };

            await _unitOfWork.PasswordResetTokenRepository.Value.AddAsync(resetToken);

            // 4. تبعت الإيميل
            await _emailService.SendAsync(user.Email, "Password Reset OTP", $"Your OTP is: {otp}");

            return new BaseResponse("OTP sent to your email.");
        }
    }
}
