using Application.Contracts;

using Domain.Entites;

using Infrastructure.Contracts_Implemintaion;
using Infrastructure.Persistence;

using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Repos
{
    public class UnitOfWork(ApplicationDbContext context, UserManager<User> userManager) : IUnitOfWork
    {
        private readonly ApplicationDbContext _context = context;

        private IGenericRepo<Product>? _products;
        private IGenericRepo<Category>? _categories;
        private IGenericRepo<Supplier>? _suppliers;
        private IGenericRepo<StockTransaction>? _stockTransactions;
        private IUserRepo _UserRepo;
        private readonly UserManager<User> _userManager = userManager;

        public IGenericRepo<Product> Products => _products ??= new GenericRepo<Product>(_context);
        public IGenericRepo<Category> Categories => _categories ??= new GenericRepo<Category>(_context);
        public IGenericRepo<Supplier> Suppliers => _suppliers ??= new GenericRepo<Supplier>(_context);
        public IGenericRepo<StockTransaction> StockTransactions => _stockTransactions ??= new GenericRepo<StockTransaction>(_context);
        public IUserRepo UserRepo => _UserRepo ??= new UserRepo(_context, _userManager);

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}