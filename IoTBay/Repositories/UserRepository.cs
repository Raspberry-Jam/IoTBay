using IoTBay.Models;
using IoTBay.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace IoTBay.Repositories;

public class UserRepository(AppDbContext db) : BaseRepository<User>(db)
{
    public async Task<User?> GetByEmail(string email)
    {
        var query = from u in _db.Users
            where u.Contact.Email == email
            select u;
        return await query.Include(u => u.Contact).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Product>> GetUserCartProducts(User user)
    {
        var query = from c in _db.UserCartProducts
            where c.User == user
            select c.Product;

        return await query.ToListAsync();
    }

    public async Task<User?> LoadFullUser(User user) => await LoadFullUser(user.UserId);

    public async Task<User?> LoadFullUser(int userId)
    {
        var query = from u in _db.Users
            where u.UserId == userId
            select u;

        return await query
            .Include(u => u.Contact)
            .Include(u => u.Orders)
            .Include(u => u.PaymentMethods)
            .Include(u => u.ShipmentMethods)
            .Include(u => u.UserAccessEvents)
            .Include(u => u.UserCartProducts)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<UserAccessEvent>> GetUserAccessEvents(User user)
    {
        var query = from e in _db.UserAccessEvents
            where e.User == user
            select e;

        return await query.ToListAsync();
    }
}