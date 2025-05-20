using IoTBay.Models;
using IoTBay.Models.Entities;

namespace IoTBay.Repositories;

public class OrderRepository(AppDbContext db) : BaseRepository<Order>(db);