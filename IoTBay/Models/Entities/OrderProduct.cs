using System;
using System.Collections.Generic;

namespace IoTBay.Models.Entities;

public partial class OrderProduct
{
    public int OrderId { get; }

    public int ProductId { get; }

    public int Quantity { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
