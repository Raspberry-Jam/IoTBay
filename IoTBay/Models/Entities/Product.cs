using System;
using System.Collections.Generic;

namespace IoTBay.Models.Entities;

public partial class Product
{
    public int ProductId { get; set; }

    public string Name { get; set; } = null!;

    public string Type { get; set; } = null!;

    public double? Price { get; set; }

    public int Stock { get; set; }

    public string? ShortDescription { get; set; }

    public string? FullDescription { get; set; }

    public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();

    public virtual ICollection<UserCartProduct> UserCartProducts { get; set; } = new List<UserCartProduct>();
}
