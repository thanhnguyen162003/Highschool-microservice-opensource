using Domain.Models.Common;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Linq.Expressions;
using System.Text.Json;

namespace Infrastructure.Repositories.GenericRepository
{

    public class RedisRepository<T> : IRedisRepository<T> where T : class
    {
        protected readonly IDatabase _database;
        private readonly string _keyPrefix; // Prefix for keys

        public RedisRepository(IDatabase database, string keyPrefix)
        {
            _database = database;
            _keyPrefix = keyPrefix;
        }

        protected string GetKey(string id) => $"{_keyPrefix}:{id}";

        public async Task Add(T entity)
        {
            var idProperty = typeof(T).GetProperty("Id");
            if (idProperty == null)
            {
                throw new InvalidOperationException($"Type {typeof(T).Name} does not have an 'Id' property.");
            }

            var idValue = idProperty.GetValue(entity);
            if (idValue == null)
            {
                throw new ArgumentNullException(nameof(idValue), "Entity.Id cannot be null.");
            }

            var redisKey = GetKey((string)idValue);
            var value = JsonSerializer.Serialize(entity);

            await _database.StringSetAsync(redisKey, value);
        }

        public async Task Delete(dynamic id)
        {
            var redisKey = GetKey(id);
            await _database.KeyDeleteAsync(redisKey);
        }

        public async Task<T?> GetById(dynamic id)
        {
            var redisKey = GetKey(id);
            var value = await _database.StringGetAsync(redisKey);

            if (value.IsNullOrEmpty)
                return null;

            return JsonSerializer.Deserialize<T>(value!);
        }

        public async Task Update(T entity)
        {
            await Add(entity);
        }

        private async Task<IQueryable<T>> GetAllAsync()
        {
            var server = _database.Multiplexer.GetServer(_database.Multiplexer.GetEndPoints().First());
            var keys = server.Keys(pattern: $"{_keyPrefix}:*");

            var tasks = keys.Select(async key =>
            {
                var value = await _database.StringGetAsync(key);
                if (!value.IsNullOrEmpty)
                {
                    var entity = JsonSerializer.Deserialize<T>(value!);
                    return entity;
                }
                return default(T);
            });

            var result = await Task.WhenAll(tasks);

            // Use OfType<T> to filter out null or default values
            return result.OfType<T>().AsQueryable<T>();
        }

        public async Task<IEnumerable<T>> GetAll(params Expression<Func<T, object>>[]? includeProperties)
        {
            var query = await GetAllAsync();

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            return await query.ToListAsync();
        }

        public async Task<PagedList<T>> GetAll(int page, int eachPage, params Expression<Func<T, object>>[]? includeProperties)
        {
            var query = await GetAllAsync();

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            return await query.ToPagedListAsync(page, eachPage);
        }

        public async Task<PagedList<T>> GetAll(Expression<Func<T, bool>> predicate,
                                            int page, int eachPage,
                                            params Expression<Func<T, object>>[]? includeProperties)
        {
            var query = await GetAllAsync();
            query = query.Where(predicate);

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            return await query.ToPagedListAsync(page, eachPage);
        }

        public async Task<PagedList<T>> GetAll(int page, int eachPage,
                                            string sortBy, bool isAscending = false,
                                            params Expression<Func<T, object>>[]? includeProperties)
        {
            var query = await GetAllAsync();

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            var result = query.ToPaginateAndSort(page, eachPage, sortBy, isAscending);

            return await Task.FromResult(result);

        }

        public async Task<PagedList<T>> GetAll(Expression<Func<T, bool>> predicate,
                                                int page, int eachPage,
                                                string sortBy, bool isAscending = true,
                                                params Expression<Func<T, object>>[]? includeProperties)
        {
            var query = await GetAllAsync();
            query = query.Where(predicate);

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            var result = query.ToPaginateAndSort(page, eachPage, sortBy, isAscending);

            return await Task.FromResult(result);

        }

    }
}
