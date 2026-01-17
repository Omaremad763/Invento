using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Application.Contracts;

using Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repos
{

    public class GenericRepo<T> : IGenericRepo<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepo(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.AsNoTracking().ToListAsync();

        public async Task<T?> GetByIdAsync(Guid id) => await _dbSet.FindAsync(id);

        public void Remove(T entity) => _dbSet.Remove(entity);

        public void Update(T entity) =>_dbSet.Update(entity);
    }

}
