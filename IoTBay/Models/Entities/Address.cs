using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace IoTBay.Models.Entities;

public enum State
{
    ACT, NSW, NT, QLD, SA, TAS, VIC, WA
}

public partial class Address
{
    public int AddressId { get; set; }

    public string? StreetLine1 { get; set; }

    public string? StreetLine2 { get; set; }

    public string? Suburb { get; set; }

    [Column("state")]
    public State? State { get; set; }

    public string? Postcode { get; set; }

    public virtual ICollection<ShipmentMethod> ShipmentMethods { get; set; } = new List<ShipmentMethod>();

    public virtual ICollection<Supplier> Suppliers { get; set; } = new List<Supplier>();
}
