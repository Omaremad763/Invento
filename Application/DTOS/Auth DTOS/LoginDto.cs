namespace Application.DTOS.Auth_DTOS
{
    public record LoginDto
    (
        string Email,
        string Password,
        string CaptachaToken
    );
    public record LoginResponse
        (
            bool IsAuthenticated,
            string Token
        );
}