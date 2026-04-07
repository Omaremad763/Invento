namespace Application.DTOS.Auth_DTOS
{
    public record SendEmailDto
(
    string UserName,
    string Link
);

    public record ConfirmEmailDto
    (
        Guid UserID,
        string Token
    );
    public record ConfirmResponse
    (
        bool IsAuthenticated,
        string ConfirmMessageResult
    );
}