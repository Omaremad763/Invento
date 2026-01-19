using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOS
{
    public record GetProductsDTO(
      string Name,
      string SKU,
      decimal Price,
      int StockQuantity,
      string CategoryName
  );
}
