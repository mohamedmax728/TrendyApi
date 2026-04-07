using Application.Common.Models;
using Application.Contracts.Persistence;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Persistence.Repositories
{
    public class BaseRepository<T> : IAsyncRepository<T> where T : class
    {
        protected readonly DbContext _context;
        protected readonly IConfigurationProvider _mapperConfig;

        public BaseRepository(
            DbContext context,
            IConfigurationProvider mapperConfig)
        {
            _context = context;
            _mapperConfig = mapperConfig;
        }

        public async Task<T?> GetAsync(
            Expression<Func<T, bool>> filter,
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _context.Set<T>();

            query = ApplyIncludes(query, includes);

            return await query
                .AsNoTracking()
                .FirstOrDefaultAsync(filter);
        }

        public async Task<IReadOnlyList<T>> ListAsync(
            Expression<Func<T, bool>>? filter = null,
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _context.Set<T>();

            query = ApplyIncludes(query, includes);

            if (filter != null)
                query = query.Where(filter);

            return await query
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<PaginatedResult<TResult>> ListPagedAsync<TResult>(
            int pageNumber,
            int pageSize,
            Expression<Func<T, bool>>? filter = null,
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _context.Set<T>();

            query = ApplyIncludes(query, includes);

            if (filter != null)
                query = query.Where(filter);

            var totalCount = await query.CountAsync();

            var items = await query
                .AsNoTracking()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<TResult>(_mapperConfig)
                .ToListAsync();

            return new PaginatedResult<TResult>(
                items,
                totalCount,
                pageNumber,
                pageSize);
        }

        public async Task<bool> AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        private static IQueryable<T> ApplyIncludes(
            IQueryable<T> query,
            Expression<Func<T, object>>[] includes)
        {
            if (includes == null || includes.Length == 0)
                return query;

            foreach (var include in includes)
                query = query.Include(include);

            return query;
        }
    }
}
