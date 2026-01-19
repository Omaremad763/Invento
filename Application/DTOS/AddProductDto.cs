using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOS
{
    public record AddProductDto(
      string Name,
      string SKU,
      decimal Price,
      Guid CategoryId
  );
}
