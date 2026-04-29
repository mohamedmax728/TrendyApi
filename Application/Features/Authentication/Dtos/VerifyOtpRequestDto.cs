using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Authentication.Dtos
{
    public record VerifyOtpRequestDto(string Email, string OTP);

}
