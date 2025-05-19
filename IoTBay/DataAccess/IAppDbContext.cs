using IoTBay.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace IoTBay.DataAccess;

public interface IAppDbContext : IDisposable
{
    DbSet<Address> Addresses { get; set; }
    DbSet<Contact> Contacts { get; set; }
    DbSet<Order> Orders { get; set; }
    DbSet<OrderProduct> OrderProducts { get; set; }
    DbSet<PaymentMethod> PaymentMethods { get; set; }
    DbSet<Product> Products { get; set; }
    DbSet<ShipmentMethod> ShipmentMethods { get; set; }
    DbSet<User> Users { get; set; }
    DbSet<UserAccessEvent> UserAccessEvents { get; set; }
    DbSet<UserCartProduct> UserCartProducts { get; set; }

    int SaveChanges();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    
    EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
    EntityEntry<TEntity> Attach<TEntity> (TEntity entity) where TEntity : class;

    DbSet<TEntity> Set<TEntity>() where TEntity : class;
}