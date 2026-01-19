using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Application.Contracts;
using Application.DTOS;

using FluentValidation;

using MediatR;

namespace Application.CQRS;

//commands
public record AddStockCommand(StockTransactionDto StockTransaction) : IRequest<bool>;

//validators
public class AddStocktValidator : AbstractValidator<AddStockCommand>
{
    public AddStocktValidator()
    {
        RuleFor(x => x.StockTransaction.ProductId).NotEmpty().NotEqual(Guid.Empty).WithMessage("Product ID is required for updates.");
        RuleFor(x => x.StockTransaction.Quantity).NotEmpty().NotEqual(0).WithMessage("Quantity Cannot be zero required for updates.");
        RuleFor(x => x.StockTransaction.TransactionType).IsInEnum().WithMessage(errorMessage: "enter valid TransactionType");

    }
}

//handlers

public class StockHandlers :
    IRequestHandler<AddStockCommand, bool>
{
    private readonly IInventoServices _service;
    public StockHandlers(IInventoServices service)
    {
        _service = service;
    }

    public async Task<bool> Handle(AddStockCommand req, CancellationToken ct)
    => await _service.StockService.AddStockAsync(req.StockTransaction);
}


