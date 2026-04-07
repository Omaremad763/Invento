namespace Application.DTOS.Auth_DTOS
{
    public record RegisterDto
    (
        string Email,
        string Password,
        string UserName
    );
    public record RegisterResponse
    (
       bool IsAuthenticated,
      string ConfirmMessageRequest
    );
}