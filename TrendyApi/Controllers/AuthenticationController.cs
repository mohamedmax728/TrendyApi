using Application.Features.Authentication;
using Application.Features.Authentication.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace TrendyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDto registerRequestDto)
        {
            var result = await _authenticationService.RegisterAsync(registerRequestDto);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto loginRequestDto)
        {
            var result = await _authenticationService.LoginAsync(loginRequestDto);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequestDto request)
        {
            var result = await _authenticationService.ForgotPasswordAsync(request);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp(VerifyOtpRequestDto request)
        {
            var result = await _authenticationService.VerifyOtpAsync(request);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequestDto request)
        {
            var result = await _authenticationService.ResetPasswordAsync(request);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }
    }
}