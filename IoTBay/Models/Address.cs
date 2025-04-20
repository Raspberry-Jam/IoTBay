using System;
using System.Collections.Generic;

namespace IoTBay.Models;

public partial class Address
{
    public int AddressId { get; set; }

    public string? StreetLine1 { get; set; }

    public string? StreetLine2 { get; set; }

    public string? Suburb { get; set; }

    public string? Postcode { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Store> Stores { get; set; } = new List<Store>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
