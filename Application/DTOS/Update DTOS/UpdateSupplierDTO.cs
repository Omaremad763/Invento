using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOS.Update_DTOS
{
    public record UpdateSupplierDTO(
        Guid id,
        string Name,
        string ContactEmail,
        string PhoneNumber
    );

}
