using IoTBay.Models;
using IoTBay.Models.Entities;

namespace IoTBay.Repositories;

public class PaymentMethodRepository(AppDbContext db) : BaseRepository<PaymentMethod>(db);