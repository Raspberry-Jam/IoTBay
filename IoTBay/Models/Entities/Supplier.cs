using System;
using System.Collections.Generic;

namespace IoTBay.Models.Entities;

public partial class Supplier
{
    public int SupplierId { get; }

    public int ContactId { get; set; }

    public int AddressId { get; set; }

    public string CompanyName { get; set; } = null!;

    public virtual Address Address { get; set; } = null!;

    public virtual Contact Contact { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
