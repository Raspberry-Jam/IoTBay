using IoTBay.DataAccess.Interfaces;
using IoTBay.Models.Entities;

namespace IoTBay.DataAccess.Implementations;

public class ContactRepository(IAppDbContext db) : BaseRepository<Contact>(db), IContactRepository;