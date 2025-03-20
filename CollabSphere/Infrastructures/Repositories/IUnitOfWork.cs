using CollabSphere.Common;

namespace CollabSphere.Infrastructures.Repositories;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync();
    Task RollBackChangesAsync();
    IBaseRepository<T> Repository<T>() where T : BaseEntity;
}
