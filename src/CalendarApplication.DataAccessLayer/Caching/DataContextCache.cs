using CalendarApplication.DataAccessLayer.Entities.Common;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace CalendarApplication.DataAccessLayer.Caching;

public class DataContextCache : IDataContextCache
{
    private readonly IMemoryCache cache;
    private readonly ILogger<DataContextCache> logger;

    public DataContextCache(IMemoryCache cache, ILogger<DataContextCache> logger)
    {
        this.cache = cache;
        this.logger = logger;
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("removing entity from cache");
        cache.Remove(id);

        return Task.CompletedTask;
    }

    public Task<T> GetAsync<T>(Guid id, CancellationToken cancellationToken = default) where T : BaseEntity
    {
        logger.LogInformation("attempting to get the entity from cache");

        if (cache.TryGetValue<T>(id, out var entity))
        {
            return Task.FromResult(entity);
        }

        return Task.FromResult<T>(null);
    }

    public Task SetAsync<T>(T entity, TimeSpan expirationTime, CancellationToken cancellationToken = default) where T : BaseEntity
    {
        logger.LogInformation("saving entity in the cache");
        cache.Set(entity.Id, entity, expirationTime);

        return Task.CompletedTask;
    }
}