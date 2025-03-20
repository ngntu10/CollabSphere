using CollabSphere.Common;
using CollabSphere.Database;
using CollabSphere.Entities.Exceptions;
using CollabSphere.Infrastructures.Specifications;

using Microsoft.EntityFrameworkCore;

namespace CollabSphere.Infrastructures.Repositories.Impl;

public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseEntity
{
    private readonly DatabaseContext Context;
    private readonly DbSet<TEntity> DbSet;

    public BaseRepository(DatabaseContext context)
    {
        Context = context;
        DbSet = context.Set<TEntity>();
    }
    public async Task<List<TEntity>> GetAllAsync(ISpecification<TEntity> spec)
    {
        return await ApplySpecification(spec).ToListAsync();
    }

    public async Task<TEntity> GetFirstOrThrowAsync(ISpecification<TEntity> spec)
    {
        var entity = await ApplySpecification(spec).FirstOrDefaultAsync();

        if (entity == null) throw new ResourceNotFoundException(typeof(TEntity));

        return entity;
    }

    public async Task<TEntity?> GetFirstAsync(ISpecification<TEntity> spec)
    {
        return await ApplySpecification(spec).FirstOrDefaultAsync();
    }

    public async Task<int> CountAsync(ISpecification<TEntity> spec)
    {
        return await ApplySpecification(spec).CountAsync();
    }

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        var addedEntity = (await DbSet.AddAsync(entity)).Entity;
        await Context.SaveChangesAsync();

        return addedEntity;
    }
    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        DbSet.Update(entity);
        await Context.SaveChangesAsync();

        return entity;
    }

    public async Task<TEntity> DeleteAsync(TEntity entity)
    {
        var removedEntity = DbSet.Remove(entity).Entity;
        await Context.SaveChangesAsync();

        return removedEntity;
    }

    private IQueryable<TEntity> ApplySpecification(ISpecification<TEntity> spec)
    {
        return SpecificationEvaluator<TEntity>.GetQuery(Context.Set<TEntity>().AsQueryable(), spec);
    }
}
