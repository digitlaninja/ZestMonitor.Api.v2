using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ZestMonitor.Api.Data.Abstract.Interfaces;

namespace ZestMonitor.Api.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected DbContext Context { get; }

        public Repository(DbContext context)
        {
            this.Context = context ??
                throw new ArgumentNullException(nameof(context));
        }

        public async Task<TEntity> Get(int id)
        {
            return await this.Context.Set<TEntity>().FindAsync();
        }

        public async Task<IEnumerable<TEntity>> GetAll()
        {
            return await this.Context.Set<TEntity>().ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return await this.Context.Set<TEntity>().Where(predicate).ToListAsync();
        }

        public async void Add(TEntity entity)
        {
            await this.Context.Set<TEntity>().AddAsync(entity);
            await this.Context.SaveChangesAsync();
        }

        public async void AddRange(IEnumerable<TEntity> entities)
        {
            await this.Context.Set<TEntity>().AddRangeAsync(entities);
            await this.Context.SaveChangesAsync();
        }

        public async void Remove(int id)
        {
            var entity = this.Get(id);
            this.Context.Remove(entity);
            await this.Context.SaveChangesAsync();
        }

        public async void Remove(TEntity entity)
        {
            this.Context.Remove(entity);
            await this.Context.SaveChangesAsync();
        }

        public async void RemoveRange(IEnumerable<TEntity> entities)
        {
            this.Context.RemoveRange(entities);
            await this.Context.SaveChangesAsync();
        }
    }
}