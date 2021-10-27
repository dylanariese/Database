using Database.Data;
using Database.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Database.Core
{
    public class Service<T> : IService<T> where T : class
    {
        private readonly DbSet<T> DbSet;
        private readonly DatabaseDbContext dbContext;

        public Service(DatabaseDbContext dbContext)
        {
            this.dbContext = dbContext;

            DbSet = dbContext.Set<T>();
        }

        public virtual async Task AddAsync(T entity)
        {
            var entry = dbContext.Entry(entity);

            if (entry.State == EntityState.Detached)
            {
                await DbSet.AddAsync(entity);
            }
            else
            {
                entry.State = EntityState.Added;
            }
        }

        public virtual async Task DeleteAsync(T entity)
        {
            await Task.Run(() =>
            {
                var entry = dbContext.Entry(entity);

                if (entry.State != EntityState.Deleted)
                {
                    entry.State = EntityState.Deleted;
                }
                else
                {
                    DbSet.Attach(entity);

                    DbSet.Remove(entity);
                }
            });
        }

        public virtual async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);

            if (entity == null)
            {
                return;
            }

            await DeleteAsync(entity);
        }

        public virtual IQueryable<T> GetAll(Expression<Func<T, bool>> predicate = null)
        {
            return predicate != null ? DbSet.Where(predicate) : DbSet;
        }

        public virtual IQueryable<T> GetAll(Func<IQueryable<T>, IIncludableQueryable<T, object>> include)
        {
            var query = DbSet as IQueryable<T>;

            query = include(query);

            return query;
        }

        public virtual IQueryable<T> GetAll(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null)
        {
            var query = DbSet as IQueryable<T>;

            if (include != null)
            {
                query = include(query);
            }

            query = query.Where(predicate);

            return query;
        }

        public virtual async Task<T> GetByIdAsync(params object[] keyValues)
        {
            return await DbSet.FindAsync(keyValues);
        }

        public virtual T GetById(int id, params Expression<Func<T, object>>[] relations)
        {
            foreach (var relation in relations)
            {
                DbSet.Include(relation).Load();
            }

            return GetById(id);
        }

        public virtual T GetSingleBy(Expression<Func<T, bool>> predicate)
        {
            return DbSet.FirstOrDefault(predicate);
        }

        public virtual T GetSingleBy(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null)
        {
            var query = DbSet as IQueryable<T>;

            if (include != null)
            {
                query = include(query);
            }

            return query.FirstOrDefault(predicate);
        }

        public virtual async Task<T> GetSingleByAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null)
        {
            var query = DbSet as IQueryable<T>;

            if (include != null)
            {
                query = include(query);
            }

            return await query.FirstOrDefaultAsync(predicate);
        }

        public virtual async Task UpdateAsync(T entity)
        {
            await Task.Run(() =>
            {
                var entry = dbContext.Entry(entity);

                if (entry.State == EntityState.Detached)
                {
                    DbSet.Attach(entity);
                }

                entry.State = EntityState.Modified;
            });
        }
    }
}