namespace Application.Features.Authentication.Dtos
{
    public record LoginRequestDto(
        string Email,
        string Password
        );
}
