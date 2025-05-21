using IoTBay.Models;
using IoTBay.Models.Entities;

namespace IoTBay.Repositories;

public class ContactRepository(AppDbContext db) : BaseRepository<Contact>(db);