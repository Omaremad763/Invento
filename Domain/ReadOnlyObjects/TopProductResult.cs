using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ReadOnlyObjects
{
    public class TopProductResult
    {
        public Guid ProductId { get; init; }
        public string ProductName { get; init; } = default!;
        public int TotalSoldQuantity { get; init; }
    }
}
