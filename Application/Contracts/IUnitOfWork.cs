using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
