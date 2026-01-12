using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Application.Contracts;

using Domain.Entites;

using Infrastructure.Persistence;

namespace Infrastructure.Repos
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        // Repositories backing fields
        private IGenericRepo<Product>? _products;
        private IGenericRepo<Category>? _categories;
        private IGenericRepo<Supplier>? _suppliers;
        private IGenericRepo<StockTransaction>? _stockTransactions;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        // Properties from IUnitOfWork
        public IGenericRepo<Product> Products => _products ??= new GenericRepo<Product>(_context);
        public IGenericRepo<Category> Categories => _categories ??= new GenericRepo<Category>(_context);
        public IGenericRepo<Supplier> Suppliers => _suppliers ??= new GenericRepo<Supplier>(_context);
        public IGenericRepo<StockTransaction> StockTransactions => _stockTransactions ??= new GenericRepo<StockTransaction>(_context);

        // Commit all changes
        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        // Dispose DbContext
        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }

}
