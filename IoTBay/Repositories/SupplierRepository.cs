using IoTBay.Models;
using IoTBay.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace IoTBay.Repositories;

public class SupplierRepository(AppDbContext db) : BaseRepository<Supplier>(db)
{
    public async Task<IEnumerable<Supplier>> GetAllNoFallback()
    {
        var query = from s in db.Suppliers
            where s.SupplierId >= 1
            select s;
        return await query.ToListAsync();
    }
}