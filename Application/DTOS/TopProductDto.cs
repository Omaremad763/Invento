using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOS
{
    public record TopProductDto(
        Guid ProductId,
        string ProductName,
        int TotalSoldQuantity);
}
