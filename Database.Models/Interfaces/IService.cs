using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Database.Models.Interfaces
{
    public interface IService<T>
    {
        Task AddAsync(T entity);

        Task DeleteAsync(T entity);

        Task DeleteAsync(int id);

        IQueryable<T> GetAll(Expression<Func<T, bool>> predicate = null);

        IQueryable<T> GetAll(Func<IQueryable<T>, IIncludableQueryable<T, object>> include);

        IQueryable<T> GetAll(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null);

        Task<T> GetByIdAsync(params object[] keyValues);

        T GetById(int id, params Expression<Func<T, object>>[] relations);

        T GetSingleBy(Expression<Func<T, bool>> predicate);

        T GetSingleBy(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null);

        Task<T> GetSingleByAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null);

        Task UpdateAsync(T entity);
    }
}