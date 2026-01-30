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

//queries
public record GetStockssQuery(ResourceParameters Parameters) : IRequest<PaginatedResult<GetStockTransactionDto>>;
public record GetProductsLookupQuery() : IRequest<IEnumerable<GetProductsLookUpDTO>>;


//commands
public record AddStockCommand(AddStockTransactionDto StockTransaction) : IRequest<bool>;
public record DeleteStockTransactionCommand(Guid Id) : IRequest<bool>;


//validators
public class AddStocktValidator : AbstractValidator<AddStockCommand>
{
    public AddStocktValidator()
    {
        RuleFor(x => x.StockTransaction.ProductId).NotEmpty().NotEqual(Guid.Empty).WithMessage("Product ID is required for updates.");
        RuleFor(x => x.StockTransaction.Quantity).NotEmpty().NotEqual(0).WithMessage("Quantity Cannot be zero required for updates.");
        RuleFor(expression: x => x.StockTransaction.TransactionType).IsInEnum().WithMessage(errorMessage: "enter valid TransactionType");

    }
}

public class DeleteStockTransactionValidator : AbstractValidator<DeleteStockTransactionCommand>
{
    public DeleteStockTransactionValidator()
    {
        RuleFor(x => x.Id).NotEqual(Guid.Empty)
            .WithMessage("A valid Product ID must be provided.");
    }
}



//handlers
public class StockHandlers :
    IRequestHandler<AddStockCommand, bool>,
        IRequestHandler<GetStockssQuery, PaginatedResult<GetStockTransactionDto>>,
        IRequestHandler<DeleteStockTransactionCommand, bool>,
        IRequestHandler<GetProductsLookupQuery, IEnumerable<GetProductsLookUpDTO>>
{
    private readonly IInventoServices _service;
    public StockHandlers(IInventoServices service)
    {
        _service = service;
    }

    public async Task<bool> Handle(AddStockCommand req, CancellationToken ct)
    => await _service.StockService.AddStockAsync(req.StockTransaction);

    public async Task<PaginatedResult<GetStockTransactionDto>>Handle(GetStockssQuery req, CancellationToken ct)
    => await _service.StockService.GetStocktransactionsAsync(req.Parameters);

    public async Task<bool> Handle(DeleteStockTransactionCommand request, CancellationToken cancellationToken)
  => await _service.StockService.SoftDeleteStockTransactionAsync(request.Id);

    public async Task<IEnumerable<GetProductsLookUpDTO>> Handle(GetProductsLookupQuery request, CancellationToken cancellationToken)
    {
        return await _service.StockService.GetProductsLookUp();
    }
}


