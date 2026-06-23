using Application.DTOS.Auth_DTOS;

namespace Application.Contracts
{
    public interface IExternalAuthService
    {
        //Task<ExternalAuthResponse> AuthGoogle(ExternalAuthDTO dto);
        Task<string> GetGitHubAccessToken(string code);

        Task<GitHubUserInfo> GetGitHubUserInfo(string accessToken);

        Task<LoginResponse> GitHubAuth(string code);
    }
}