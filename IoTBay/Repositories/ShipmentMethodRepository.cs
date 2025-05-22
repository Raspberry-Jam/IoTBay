using IoTBay.Models;
using IoTBay.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace IoTBay.Repositories;

public class ShipmentMethodRepository(AppDbContext db) : BaseRepository<ShipmentMethod>(db)
{
    public async Task<ShipmentMethod> LoadWithAddress(ShipmentMethod shipmentMethod)
    {
        var query = from sm in db.ShipmentMethods
            where sm.ShipmentMethodId == shipmentMethod.ShipmentMethodId
            select sm;

        var result = await query.Include(sm => sm.Address).FirstOrDefaultAsync();
        return result!;
    }
}