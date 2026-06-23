using Application.Contracts;
using Application.DTOS;

using AutoMapper;
using AutoMapper.QueryableExtensions;

using Domain.Entites;

using Infrastructure.Extentions;

using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Contracts_Implemintaion;

public class StockService(IMapper mapper, IUnitOfWork unitOfWork) : IStockService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    public async Task<bool> AddStockAsync(AddStockTransactionDto dto)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(dto.ProductId);
        if (product == null) return false;
        product.ChangeStock(dto.Quantity, product.StockQuantity, dto.TransactionType);

        var stockRecord = new StockTransaction(product.Id, dto.Quantity, dto.TransactionType);

        _unitOfWork.Products.Update(product);
        await _unitOfWork.StockTransactions.AddAsync(stockRecord);

        return await _unitOfWork.CommitAsync() > 0;
    }

    public async Task<PaginatedResult<GetStockTransactionDto>> GetStocktransactionsAsync(ResourceParameters parameters)
    {
        var stockTransactions = _unitOfWork.StockTransactions.GetAllWithIncludeAsync(p => p.Product);
        if (!string.IsNullOrEmpty(parameters.SearchTerm))
        {
            var isEnumValue = Enum.GetValues<StockTransactionTypeEnum>()
            .FirstOrDefault(e => e.ToString().ToLower().Contains(parameters.SearchTerm));

            // 2. بناء الـ Query
            stockTransactions = stockTransactions.Where(p =>
                p.Product.Name.ToLower().Contains(parameters.SearchTerm) ||
                (isEnumValue != 0 && p.StockTransactionType == isEnumValue));
        }
        var projectedQuery = stockTransactions.ProjectTo<GetStockTransactionDto>(_mapper.ConfigurationProvider);

        var result = await projectedQuery.ToPaginatedListAsync(parameters.PageNumber, parameters.PageSize);
        return result;
    }

    public async Task<bool> SoftDeleteStockTransactionAsync(Guid id)
    {
        StockTransaction? StockTransaction = await _unitOfWork.StockTransactions.GetByIdAsync(id);
        if (StockTransaction == null)
        {
            return false;
        }
        StockTransaction.IsDeleted = true;
        int saving = await _unitOfWork.CommitAsync();
        //return true if the saving is greater than 0
        return saving > 0;
    }

    public async Task<IEnumerable<GetProductsLookUpDto>> GetProductsLookUp()
    {
        var query = _unitOfWork.Products.GetAllAsync();

        var projectedQuery = query.ProjectTo<GetProductsLookUpDto>(_mapper.ConfigurationProvider);

        return await projectedQuery.ToListAsync();
    }
}