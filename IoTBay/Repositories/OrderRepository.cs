using IoTBay.Models;
using IoTBay.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace IoTBay.Repositories;

public class OrderRepository(AppDbContext db) : BaseRepository<Order>(db)
{
    public async Task<Order> DeepLoadOrderProducts(Order order)
    {
        var query = from o in _db.Orders
            where o.OrderId == order.OrderId
            select o;

        var result = await query
            .Include(o => o.OrderProducts)
            .ThenInclude(o => o.Product)
            .FirstOrDefaultAsync();
        
        return result!;
    }
}