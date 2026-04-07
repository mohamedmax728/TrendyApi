namespace Application.Features.Authentication.Dtos
{
    public record RegisterRequestDto(
        string FullNameEn,
        string FullNameAr,
        string Email,
        string PhoneNumber,
        string Password,
        string ConfirmPassword
        );
}
