using IoTBay.DataAccess.Interfaces;
using IoTBay.Models.Entities;

namespace IoTBay.DataAccess.Implementations;

public class ProductRepository(IAppDbContext db) : BaseRepository<Product>(db), IProductRepository;