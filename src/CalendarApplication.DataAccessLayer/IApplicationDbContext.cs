using CalendarApplication.DataAccessLayer.Entities.Common;

namespace CalendarApplication.DataAccessLayer;

public interface IApplicationDbContext
{
    Task DeleteAsync<T>(T entity) where T : BaseEntity;

    Task DeleteAsync<T>(IEnumerable<T> entities) where T : BaseEntity;

    Task<T> GetAsync<T>(Guid id) where T : BaseEntity;

    IQueryable<T> GetData<T>(bool trackingChanges = false) where T : BaseEntity;

    Task InsertAsync<T>(T entity) where T : BaseEntity;

    Task<int> SaveAsync();
}