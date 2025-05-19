using IoTBay.Models.Entities;

namespace IoTBay.DataAccess.Interfaces;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> GetByEmail(string email);
    Task<IEnumerable<Product>> GetUserCartProducts(User user);
}