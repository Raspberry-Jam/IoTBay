using IoTBay.Models;
using IoTBay.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace IoTBay.Repositories;

public class ProductRepository(AppDbContext db) : BaseRepository<Product>(db)
{
    public async Task<List<Product>> GetAllNoFallback()
    {
        var query = from p in db.Products
            where p.ProductId >= 1
            select p;
        return await query.ToListAsync();
    }
}