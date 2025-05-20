using System;
using System.Collections.Generic;

namespace IoTBay.Models.Entities;

public partial class ShipmentMethod
{
    public int ShipmentMethodId { get; set; }

    public int UserId { get; set; }

    public int AddressId { get; set; }

    public string Method { get; set; } = null!;

    public virtual Address Address { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual User User { get; set; } = null!;
}
