using IoTBay.Models;
using IoTBay.Models.Entities;

namespace IoTBay.Repositories;

public class ShipmentMethodRepository(AppDbContext db) : BaseRepository<ShipmentMethod>(db);