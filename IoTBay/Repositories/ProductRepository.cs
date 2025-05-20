using IoTBay.Models;
using IoTBay.Models.Entities;

namespace IoTBay.Repositories;

public class ProductRepository(AppDbContext db) : BaseRepository<Product>(db);