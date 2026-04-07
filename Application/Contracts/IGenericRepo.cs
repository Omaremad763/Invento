using System.Linq.Expressions;

namespace Application.Contracts
{
    public interface IGenericRepo<T> where T : class
    {
        Task<T?> GetByIdAsync(Guid id);

        IQueryable<T> GetAllAsync();

        Task AddAsync(T entity);

        void Update(T entity);

        void Remove(T entity);

        IQueryable<T> GetAllWithIncludeAsync(params Expression<Func<T, object>>[] includes);
    }
}