using IoTBay.DataAccess.Interfaces;
using IoTBay.Models.Entities;

namespace IoTBay.DataAccess.Implementations;

public class SupplierRepository(IAppDbContext db) : BaseRepository<Supplier>(db), ISupplierRepository;