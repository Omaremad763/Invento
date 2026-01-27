using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
