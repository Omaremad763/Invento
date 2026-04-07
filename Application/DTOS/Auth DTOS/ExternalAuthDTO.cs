namespace Application.DTOS.Auth_DTOS;
public record ExternalAuthDto
    (
        string Code
    );
public record ExternalAuthResponse
    (
        bool IsAuthenticated,
        string Token
    );
public record GitHubUserInfo(
    string GitHubId,
    string Name,
    string Email,
    string AvatarUrl
);
public record GitHubEmailResponse(string Email, bool Primary, bool Verified);