using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOS.Auth_DTOS;
public record ExternalAuthDTO
    (
        string code
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