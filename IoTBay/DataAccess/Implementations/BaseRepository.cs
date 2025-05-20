using IoTBay.DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IoTBay.DataAccess.Implementations;

public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
{
    protected readonly IAppDbContext _db;
    protected readonly DbSet<TEntity> _dbSet;
    
    public BaseRepository(IAppDbContext db)
    {
        _db = db;
        _dbSet = _db.Set<TEntity>();
    }
    
    public virtual async Task<TEntity> GetById(int id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity == null) throw new NullReferenceException("Entity not found");
        return entity;
    }

    public virtual async Task<IEnumerable<TEntity>> GetAll()
    {
        return await _dbSet.ToListAsync();
    }

    public virtual async Task Add(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
        await _db.SaveChangesAsync();
    }

    public virtual async Task AddRange(IEnumerable<TEntity> entities)
    {
        await _dbSet.AddRangeAsync(entities);
    }

    public virtual void Update(TEntity entity)
    {
        _dbSet.Attach(entity);
        _db.Entry(entity).State = EntityState.Modified;
    }

    public virtual async Task Delete(int id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity == null) throw new NullReferenceException("Entity not found");
        if (_db.Entry(entity).State == EntityState.Detached)
        {
            _dbSet.Attach(entity);
        }

        _dbSet.Remove(entity);
    }

    public virtual void Delete(TEntity entity)
    {
        if (_db.Entry(entity).State == EntityState.Detached)
        {
            _dbSet.Attach(entity);
        }

        _dbSet.Remove(entity);
    }

    public virtual async Task<int> SaveChangesAsync() => await _db.SaveChangesAsync();
}