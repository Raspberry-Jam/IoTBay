using System;
using System.Collections.Generic;

namespace IoTBay.Models.Entities;

public partial class PaymentMethod
{
    public int PaymentMethodId { get; set; }

    public int UserId { get; set; }

    public string CardNumber { get; set; } = null!;

    public string Cvv { get; set; } = null!;

    public DateOnly Expiry { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual User User { get; set; } = null!;
}
