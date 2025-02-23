using CollabSphere.Common;
using CollabSphere.Infrastructures.Specifications;

namespace CollabSphere.Infrastructures.Repositories;

public interface IBaseRepository<TEntity> where TEntity : BaseEntity
{
    Task<TEntity> GetFirstOrThrowAsync(ISpecification<TEntity> spec);

    Task<TEntity?> GetFirstAsync(ISpecification<TEntity> spec);

    Task<List<TEntity>> GetAllAsync(ISpecification<TEntity> spec);

    Task<int> CountAsync(ISpecification<TEntity> spec);

    Task<TEntity> AddAsync(TEntity entity);

    Task<TEntity> UpdateAsync(TEntity entity);

    Task<TEntity> DeleteAsync(TEntity entity);
}
