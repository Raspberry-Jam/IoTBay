using System;
using System.Collections.Generic;

namespace IoTBay.Models;

public partial class Catalogue
{
    public int CatalogueId { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual ICollection<Store> Stores { get; set; } = new List<Store>();
}
