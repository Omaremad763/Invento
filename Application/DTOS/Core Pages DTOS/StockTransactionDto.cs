using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Domain.Entites;

namespace Application.DTOS;

public record AddStockTransactionDto(

    Guid ProductId,
    int Quantity,
    StockTransactionTypeEnum TransactionType
);
public record GetStockTransactionDto
(
    Guid Id,
    string ProductName,
    int AppliedQuantity,
    string StockTransactionType,
    DateTime CreatedAt
);

public record GetProductsLookUpDto(
 Guid Id,
  string Name
);


