using IoTBay.DataAccess.Interfaces;
using IoTBay.Models.Entities;

namespace IoTBay.DataAccess.Implementations;

public class PaymentMethodRepository(IAppDbContext db) : BaseRepository<PaymentMethod>(db), IPaymentMethod;