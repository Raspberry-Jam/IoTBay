namespace IoTBay.DataAccess.Interfaces;

public interface IBaseRepository<TEntity> where TEntity : class
{
    /// <summary>
    /// Returns the entity object by given ID (if it exists)
    /// </summary>
    /// <param name="id">Entity primary key ID</param>
    /// <returns>The entity</returns>
    Task<TEntity> GetById(int id);
    
    /// <summary>
    /// Returns a list of all entities in this list (rows in table)
    /// </summary>
    /// <returns>List of entities</returns>
    Task<IEnumerable<TEntity>> GetAll();
    
    /// <summary>
    /// Adds a new entity to the set (inserts row into table)
    /// </summary>
    /// <param name="entity">New entity to be added</param>
    Task Add(TEntity entity);

    /// <summary>
    /// Adds a range of new entities to the set (inserts multiple rows into table)
    /// </summary>
    /// <param name="entities">List of entities to be added</param>
    Task AddRange(IEnumerable<TEntity> entities);
    
    /// <summary>
    /// Updates an existing entity's fields (updates a row's columns)
    /// </summary>
    /// <param name="entity">Existing entity to be updated</param>
    void Update(TEntity entity);
    
    /// <summary>
    /// Deletes an entity by its ID (if it exists)
    /// </summary>
    /// <param name="id">Entity primary key ID</param>
    Task Delete(int id);
    
    /// <summary>
    /// Deletes the entity from the set (drops row from table)
    /// </summary>
    /// <param name="entity">Entity to be deleted</param>
    void Delete(TEntity entity);

    Task<int> SaveChangesAsync();
}