using System;
using System.Collections.Generic;

namespace IoTBay.Models.Entities;

public partial class Order
{
    public int OrderId { get; }

    public int UserId { get; set; }

    public int ShipmentMethodId { get; set; }

    public int PaymentMethodId { get; set; }

    public DateOnly OrderDate { get; set; }

    public DateOnly? SentDate { get; set; }

    public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();

    public virtual PaymentMethod PaymentMethod { get; set; } = null!;

    public virtual ShipmentMethod ShipmentMethod { get; set; } = null!;

    public virtual User User { get; set; } = null!;
    
    public virtual Shipment? Shipment { get; set; }
}
