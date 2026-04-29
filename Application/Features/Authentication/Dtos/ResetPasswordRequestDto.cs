using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Authentication.Dtos
{
    public record ResetPasswordRequestDto(string Email, string OTP, string NewPassword, string ConfirmPassword);

}
