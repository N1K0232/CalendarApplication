using System.Reflection;
using CalendarApplication.Authentication;
using CalendarApplication.DataAccessLayer.Caching;
using CalendarApplication.DataAccessLayer.Entities.Common;
using EntityFramework.Exceptions.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CalendarApplication.DataAccessLayer;

public class ApplicationDbContext : AuthenticationDbContext, IApplicationDbContext
{
    private readonly IDataContextCache cache;
    private readonly ILogger<ApplicationDbContext> logger;
    private CancellationTokenSource cancellationTokenSource;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
        IDataContextCache cache,
        ILogger<ApplicationDbContext> logger) : base(options)
    {
        this.cache = cache;
        this.logger = logger;

        cancellationTokenSource = new CancellationTokenSource();
    }

    public async Task DeleteAsync<T>(T entity) where T : BaseEntity
    {
        await cache.DeleteAsync(entity.Id, cancellationTokenSource.Token);

        logger.LogInformation("prepare entity for being deleted");
        Set<T>().Remove(entity);
    }

    public async Task DeleteAsync<T>(IEnumerable<T> entities) where T : BaseEntity
    {
        foreach (var entity in entities)
        {
            await cache.DeleteAsync(entity.Id, cancellationTokenSource.Token);
        }

        logger.LogInformation("preparing entities for being deleted");
        Set<T>().RemoveRange(entities);
    }

    public async Task<T> GetAsync<T>(Guid id) where T : BaseEntity
    {
        var cachedEntity = await cache.GetAsync<T>(id, cancellationTokenSource.Token);
        if (cachedEntity is not null)
        {
            return cachedEntity;
        }

        var entity = await Set<T>().FindAsync([id], cancellationTokenSource.Token);
        return entity;
    }

    public IQueryable<T> GetData<T>(bool trackingChanges = false) where T : BaseEntity
    {
        var set = Set<T>();
        return trackingChanges ? set.AsTracking() : set.AsNoTrackingWithIdentityResolution();
    }

    public async Task InsertAsync<T>(T entity) where T : BaseEntity
    {
        await cache.SetAsync(entity, TimeSpan.FromHours(1), cancellationTokenSource.Token);

        logger.LogInformation("preparing entity for being inserted");
        await Set<T>().AddAsync(entity, cancellationTokenSource.Token);
    }

    public async Task<int> SaveAsync()
    {
        logger.LogInformation("attempting to save changes in the database");

        var entries = ChangeTracker.Entries()
            .Where(e => typeof(BaseEntity).IsAssignableFrom(e.Entity.GetType())).ToList();

        foreach (var entry in entries.Where(e => e.State is EntityState.Added or EntityState.Modified))
        {
            var entity = entry.Entity as BaseEntity;

            if (entry.State is EntityState.Modified)
            {
                entity.LastModificationDate = DateTime.UtcNow;
            }
        }

        var affectedRows = await SaveChangesAsync(true, cancellationTokenSource.Token);
        logger.LogInformation("saved {affectedRows} rows in the database", affectedRows);

        return affectedRows;
    }

    public override void Dispose()
    {
        if (cancellationTokenSource is not null)
        {
            cancellationTokenSource.Dispose();
            cancellationTokenSource = null;
        }

        base.Dispose();
        GC.SuppressFinalize(this);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseExceptionProcessor();
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        var assembly = Assembly.GetExecutingAssembly();
        builder.ApplyConfigurationsFromAssembly(assembly);

        base.OnModelCreating(builder);
    }
}