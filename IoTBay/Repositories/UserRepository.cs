using IoTBay.Models;
using IoTBay.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace IoTBay.Repositories;

public class UserRepository(AppDbContext db) : BaseRepository<User>(db)
{
    /// <summary>
    /// Searches the list of users by email, and returns one if there is a match
    /// </summary>
    /// <param name="email">User email</param>
    /// <returns>User if it exists, or null if not</returns>
    public async Task<User?> GetByEmail(string email)
    {
        var query = from u in _db.Users
            where u.Contact.Email == email
            select u;
        return await query
            .Include(u => u.Contact)
            .Include(u => u.UserAccessEvents) // Get access events for login by email method
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Get a list of all the products in the user's cart
    /// </summary>
    /// <param name="user">User to get cart of</param>
    /// <returns>List of products</returns>
    public async Task<IEnumerable<Product>> GetUserCartProducts(User user)
    {
        var query = from c in _db.UserCartProducts
            where c.User == user
            select c.Product;

        return await query.ToListAsync();
    }

    /// <summary>
    /// Loads all the navigation properties of a given user
    /// </summary>
    /// <param name="user">User to eager load</param>
    /// <returns>User if it exists, null if not</returns>
    public async Task<User?> LoadFullUser(User user) => await LoadFullUser(user.UserId);

    /// <summary>
    /// Retrieve the user from the database and eager-load all of its navigation properties
    /// </summary>
    /// <param name="userId">ID of user to eager load</param>
    /// <returns>User if it exists, null if not</returns>
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

    /// <summary>
    /// Get a list of all the User Access Events for a given user.
    /// </summary>
    /// <param name="user">User with access events</param>
    /// <returns>List of user access events</returns>
    public async Task<IEnumerable<UserAccessEvent>> GetUserAccessEvents(User user)
    {
        var query = from e in _db.UserAccessEvents
            where e.User == user
            select e;

        return await query.ToListAsync();
    }
}