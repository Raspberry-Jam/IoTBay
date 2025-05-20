using IoTBay.DataAccess.Interfaces;
using IoTBay.Models.Entities;

namespace IoTBay.DataAccess.Implementations;

public class AddressRepository(IAppDbContext db) : BaseRepository<Address>(db), IAddressRepository;