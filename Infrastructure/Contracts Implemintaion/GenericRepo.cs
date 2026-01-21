using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using Application.Contracts;
using Application.DTOS;

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

        public IQueryable<T> GetAllAsync() => _dbSet.AsNoTracking().AsQueryable();
        public async Task<T?> GetByIdAsync(Guid id) => await _dbSet.FindAsync(id);

        public void Remove(T entity) => _dbSet.Remove(entity);

        public void Update(T entity) =>_dbSet.Update(entity);

        public IQueryable<T> GetAllWithIncludeAsync(params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return query.AsNoTracking();
        }
    }

}
