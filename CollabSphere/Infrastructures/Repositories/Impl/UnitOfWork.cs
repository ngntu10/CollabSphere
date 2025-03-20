using CollabSphere.Common;
using CollabSphere.Database;

namespace CollabSphere.Infrastructures.Repositories.Impl;

public class UnitOfWork : IUnitOfWork
{
    private readonly DatabaseContext _dbContext;
    private readonly IDictionary<Type, dynamic> _repositories;

    public UnitOfWork(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
        _repositories = new Dictionary<Type, dynamic>();
    }

    public IBaseRepository<T> Repository<T>() where T : BaseEntity
    {
        var entityType = typeof(T);

        if (_repositories.ContainsKey(entityType))
        {
            return _repositories[entityType];
        }

        var repositoryType = typeof(BaseRepository<>);

        var repository = Activator.CreateInstance(repositoryType.MakeGenericType(entityType), _dbContext);

        if (repository == null)
        {
            throw new NullReferenceException("Repository should not be null");
        }

        _repositories.Add(entityType, repository);

        return (IBaseRepository<T>) repository;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _dbContext.SaveChangesAsync();
    }

    public async Task RollBackChangesAsync()
    {
        await _dbContext.Database.RollbackTransactionAsync();
    }
}
