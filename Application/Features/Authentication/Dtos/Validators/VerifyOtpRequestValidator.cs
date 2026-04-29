using FluentValidation;

namespace Application.Features.Authentication.Dtos.Validators
{
    public class VerifyOtpRequestValidator : AbstractValidator<VerifyOtpRequestDto>
    {
        public VerifyOtpRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .NotNull()
                .EmailAddress();

            RuleFor(x => x.OTP)
                .NotEmpty()
                .NotNull()
                .Length(6, 6)
                .Matches("^[0-9]*$").WithMessage("OTP must contain only digits.");
        }
    }
}
