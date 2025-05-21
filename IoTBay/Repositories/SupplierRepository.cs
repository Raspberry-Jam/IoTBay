using IoTBay.Models;
using IoTBay.Models.Entities;

namespace IoTBay.Repositories;

public class SupplierRepository(AppDbContext db) : BaseRepository<Supplier>(db);