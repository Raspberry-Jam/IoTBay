using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace IoTBay.Models.Entities;

public enum Role
{
    Customer, Staff, System
}

public partial class User
{
    public int UserId { get; set; }
    
    [Column("role")]
    public Role Role { get; set; }

    public string? PasswordHash { get; set; }

    public string? PasswordSalt { get; set; }

    public int? ContactId { get; set; }

    public virtual Contact? Contact { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<PaymentMethod> PaymentMethods { get; set; } = new List<PaymentMethod>();

    public virtual ICollection<ShipmentMethod> ShipmentMethods { get; set; } = new List<ShipmentMethod>();

    public virtual ICollection<UserAccessEvent> UserAccessEvents { get; set; } = new List<UserAccessEvent>();

    public virtual ICollection<UserCartProduct> UserCartProducts { get; set; } = new List<UserCartProduct>();
}
