using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ZestMonitor.Api.Data.Abstract.Interfaces;

namespace ZestMonitor.Api.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected DbContext Context { get; }

        public Repository(DbContext context)
        {
            this.Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<TEntity> Get(int id)
        {
            return await this.Context.Set<TEntity>().FindAsync(id);
        }

        public IQueryable<TEntity> GetAll()
        {
            return this.Context.Set<TEntity>();
        }

        public async Task<IEnumerable<TEntity>> FindBy(Expression<Func<TEntity, bool>> predicate)
        {
            return await this.Context.Set<TEntity>().Where(predicate).ToListAsync();
        }

        public async Task Add(TEntity entity)
        {
            await this.Context.Set<TEntity>().AddAsync(entity);
        }

        public async Task AddRange(IEnumerable<TEntity> entities)
        {
            await this.Context.Set<TEntity>().AddRangeAsync(entities);
        }

        public void Remove(int id)
        {
            var entity = this.Get(id);
            this.Context.Remove(entity);
        }

        public void Remove(TEntity entity)
        {
            this.Context.Remove(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            this.Context.RemoveRange(entities);
        }

        public async Task<bool> SaveAll()
        {
            return await this.Context.SaveChangesAsync() > 0;
        }

        public bool AnyTracked() => this.Context.ChangeTracker.HasChanges();
    }
}