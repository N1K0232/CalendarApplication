using CalendarApplication.DataAccessLayer.Entities.Common;

namespace CalendarApplication.DataAccessLayer.Caching;

public interface IDataContextCache
{
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<T> GetAsync<T>(Guid id, CancellationToken cancellationToken = default) where T : BaseEntity;

    Task SetAsync<T>(T entity, TimeSpan expirationTime, CancellationToken cancellationToken = default) where T : BaseEntity;
}