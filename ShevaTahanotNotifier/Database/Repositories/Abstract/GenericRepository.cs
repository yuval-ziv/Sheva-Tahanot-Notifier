using Microsoft.EntityFrameworkCore;
using ShevaTahanotNotifier.Database.Entities;
using ShevaTahanotNotifier.ExtensionMethods;

namespace ShevaTahanotNotifier.Database.Repositories.Abstract;

public abstract class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    protected readonly NotifierContext Context;
    protected readonly DbSet<T> DbSet;
    protected IQueryable<T> Table => DbSet;
    protected IQueryable<T> TableNoTracking => DbSet.AsNoTracking();

    protected GenericRepository(NotifierContext context)
    {
        Context = context;
        DbSet = context.Set<T>();
    }

    private IQueryable<T> GetBaseQuery(bool tracking = true) => tracking ? Table : TableNoTracking;

    public IQueryable<T> GetAll(bool tracking = false)
    {
        return GetBaseQuery(tracking);
    }

    public Task<T?> GetByIdAsync(Guid id, bool tracking = false, CancellationToken cancellationToken = default)
    {
        return GetBaseQuery(tracking).FirstOrDefaultAsync(entity => entity.Id == id, cancellationToken);
    }

    public IQueryable<T> GetByIds(IEnumerable<Guid> ids, bool tracking = false)
    {
        return GetBaseQuery(tracking).Where(entity => ids.Contains(entity.Id));
    }

    public Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return GetBaseQuery(tracking: false).AnyAsync(entity => entity.Id == id, cancellationToken);
    }

    public Task<bool> ExistsAllByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        return GetBaseQuery(tracking: false).AllAsync(entity => ids.Contains(entity.Id), cancellationToken);
    }

    public Task<bool> ExistsAnyByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        return GetBaseQuery(tracking: false).AnyAsync(entity => ids.Contains(entity.Id), cancellationToken);
    }

    public async Task<T> CreateAsync(T entity, bool saveChanges = true, CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);
        if (saveChanges)
        {
            await Context.SaveChangesAsync(cancellationToken);
        }

        return entity;
    }

    public async Task<ICollection<T>> CreateAsync(IEnumerable<T> entities, bool saveChanges = true, CancellationToken cancellationToken = default)
    {
        ICollection<T> entitiesArray = entities.ToCollection();
        await DbSet.AddRangeAsync(entitiesArray, cancellationToken);
        if (saveChanges)
        {
            await Context.SaveChangesAsync(cancellationToken);
        }

        return entitiesArray;
    }

    public async Task<T> UpdateAsync(T entity, bool saveChanges = true, CancellationToken cancellationToken = default)
    {
        DbSet.Update(entity);
        if (saveChanges)
        {
            await Context.SaveChangesAsync(cancellationToken);
        }

        return entity;
    }

    public async Task<ICollection<T>> UpdateAsync(IEnumerable<T> entities, bool saveChanges = true, CancellationToken cancellationToken = default)
    {
        ICollection<T> entitiesArray = entities.ToCollection();
        DbSet.UpdateRange(entitiesArray);
        if (saveChanges)
        {
            await Context.SaveChangesAsync(cancellationToken);
        }

        return entitiesArray;
    }

    public async Task<DeleteResult> DeleteAsync(Guid id, bool saveChanges = true, CancellationToken cancellationToken = default)
    {
        T? entity = await GetByIdAsync(id, tracking: true, cancellationToken);

        if (entity is null)
        {
            return DeleteResult.NotFound;
        }

        return await DeleteAsync(entity, saveChanges, cancellationToken);
    }

    public async Task<DeleteResult> DeleteAsync(T entity, bool saveChanges = true, CancellationToken cancellationToken = default)
    {
        DbSet.Remove(entity);
        if (!saveChanges)
        {
            return DeleteResult.NotSaved;
        }

        await SaveChangesAsync(cancellationToken);
        return DeleteResult.Deleted;
    }

    public Task DeleteAsync(IEnumerable<Guid> ids, bool saveChanges = true, CancellationToken cancellationToken = default)
    {
        IQueryable<T> entities = GetByIds(ids);
        return DeleteAsync(entities, saveChanges, cancellationToken);
    }

    public async Task DeleteAsync(IEnumerable<T> entities, bool saveChanges = true, CancellationToken cancellationToken = default)
    {
        DbSet.RemoveRange(entities);
        if (saveChanges)
        {
            await SaveChangesAsync(cancellationToken);
        }
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return Context.SaveChangesAsync(cancellationToken);
    }
}