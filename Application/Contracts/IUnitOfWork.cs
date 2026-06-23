using Domain.Entites;

namespace Application.Contracts
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepo<Product> Products { get; }
        IGenericRepo<Category> Categories { get; }
        IGenericRepo<Supplier> Suppliers { get; }
        IGenericRepo<StockTransaction> StockTransactions { get; }
        IUserRepo UserRepo { get; }

        Task<int> CommitAsync();
    }
}