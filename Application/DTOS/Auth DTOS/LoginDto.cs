namespace Application.DTOS.Auth_DTOS
{
    public record LoginDto
    (
        string Email,
        string Password
    );
    public record LoginResponse
        (
            bool IsAuthenticated,
            string Token
        );
}