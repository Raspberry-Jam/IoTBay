using IoTBay.DataAccess.Interfaces;
using IoTBay.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace IoTBay.DataAccess.Implementations;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    private readonly IAppDbContext _db;
    private readonly DbSet<User> _users;

    public UserRepository(IAppDbContext db) : base(db)
    {
        _db = db;
        _users = _db.Users;
    }

    public async Task<User?> GetByEmail(string email)
    {
        var query = from u in _users
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
}