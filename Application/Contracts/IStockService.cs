using Application.DTOS;

namespace Application.Contracts
{
    public interface IStockService
    {
        Task<bool> AddStockAsync(AddStockTransactionDto dto);

        Task<PaginatedResult<GetStockTransactionDto>> GetStocktransactionsAsync(ResourceParameters parameters);

        Task<bool> SoftDeleteStockTransactionAsync(Guid id);

        Task<IEnumerable<GetProductsLookUpDto>> GetProductsLookUp();
    }
}