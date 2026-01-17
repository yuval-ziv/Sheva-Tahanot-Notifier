using ShevaTahanotNotifier.Database.Entities;

namespace ShevaTahanotNotifier.Repositories.Abstract;

public interface IGenericRepository<T> where T : BaseEntity
{
    IQueryable<T> GetAll(bool tracking = false);
    Task<T?> GetByIdAsync(Guid id, bool tracking = false, CancellationToken cancellationToken = default);
    IQueryable<T> GetByIds(IEnumerable<Guid> ids, bool tracking = false);
    Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAllByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
    Task<bool> ExistsAnyByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
    Task<T> CreateAsync(T entity, bool saveChanges = true, CancellationToken cancellationToken = default);
    Task<ICollection<T>> CreateAsync(IEnumerable<T> entities, bool saveChanges = true, CancellationToken cancellationToken = default);
    Task<T> UpdateAsync(T entity, bool saveChanges = true, CancellationToken cancellationToken = default);
    Task<ICollection<T>> UpdateAsync(IEnumerable<T> entities, bool saveChanges = true, CancellationToken cancellationToken = default);
    Task<DeleteResult> DeleteAsync(Guid id, bool saveChanges = true, CancellationToken cancellationToken = default);
    Task<DeleteResult> DeleteAsync(T entity, bool saveChanges = true, CancellationToken cancellationToken = default);
    Task DeleteAsync(IEnumerable<Guid> ids, bool saveChanges = true, CancellationToken cancellationToken = default);
    Task DeleteAsync(IEnumerable<T> entities, bool saveChanges = true, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}