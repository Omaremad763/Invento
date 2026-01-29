using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOS.Auth_DTOS
{
    public record SendEmailDto
(
    string UserName,
    string link
);

    public record ConfirmEmailDTO
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
