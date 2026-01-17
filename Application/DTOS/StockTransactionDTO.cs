using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Domain.Entites;

namespace Application.DTOS
{
public record StockTransactionDto(

    Guid ProductId,
    int Quantity,
    StockTransactionTypeEnum TransactionType
);
}
