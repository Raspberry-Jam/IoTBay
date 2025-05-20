using IoTBay.Models;
using IoTBay.Models.Entities;

namespace IoTBay.Repositories;

public class AddressRepository(AppDbContext db) : BaseRepository<Address>(db);