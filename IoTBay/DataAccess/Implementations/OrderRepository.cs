using IoTBay.DataAccess.Interfaces;
using IoTBay.Models.Entities;

namespace IoTBay.DataAccess.Implementations;

public class OrderRepository(IAppDbContext db) : BaseRepository<Order>(db), IOrderRepository;