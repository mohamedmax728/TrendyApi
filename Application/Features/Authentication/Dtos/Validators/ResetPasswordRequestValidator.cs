using FluentValidation;

namespace Application.Features.Authentication.Dtos.Validators
{
    public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequestDto>
    {
        public ResetPasswordRequestValidator()
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

            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .NotNull()
                .MinimumLength(8);

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.NewPassword)
                .WithMessage("Passwords do not match.");
        }
    }
}
