using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOS
{
    public record SupplierDto(
        Guid Id,
        string Name,
        string ContactEmail
    );
}
