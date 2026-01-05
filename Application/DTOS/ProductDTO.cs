using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOS
{
    public record ProductDto(
      Guid Id,
      string Name,
      string SKU,
      decimal Price,
      int StockQuantity,
      Guid CategoryId
  );
}
