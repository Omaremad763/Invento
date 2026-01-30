using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOS
{
    public record SupplierDto(
        Guid id,
        string Name,
        string ContactEmail,
        string PhoneNumber,
        string? Vatstatus
    );
    public record AddSupplierDTO
(
    string Name,
    string ContactEmail,
    string PhoneNumber
);

    public record UpdateSupplierDTO(
    Guid id,
    string Name,
    string ContactEmail,
    string PhoneNumber
);

}
