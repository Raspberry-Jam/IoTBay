using IoTBay.DataAccess.Interfaces;
using IoTBay.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace IoTBay.DataAccess.Implementations;

public class UserRepository(IAppDbContext db) : BaseRepository<User>(db), IUserRepository
{
    public async Task<User?> GetByEmail(string email)
    {
        var query = from u in _db.Users
            where u.Contact.Email == email
            select u;
        return await query.FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Product>> GetUserCartProducts(User user)
    {
        var query = from c in _db.UserCartProducts
            where c.User == user
            select c.Product;

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<UserAccessEvent>> GetUserAccessEvents(User user)
    {
        var query = from e in _db.UserAccessEvents
            where e.User == user
            select e;

        return await query.ToListAsync();
    }
}