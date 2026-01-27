using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
