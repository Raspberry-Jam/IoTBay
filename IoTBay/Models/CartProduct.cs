using System;
using System.Collections.Generic;

namespace IoTBay.Models;

public partial class CartProduct
{
    public int CartId { get; set; }

    public int ProductId { get; set; }

    public int Amount { get; set; }

    public virtual Cart Cart { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
