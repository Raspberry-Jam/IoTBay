using System;
using System.Collections.Generic;

namespace IoTBay.Models;

public partial class Contact
{
    public int ContactId { get; set; }

    public string GivenName { get; set; } = null!;

    public string? Surname { get; set; }

    public string Email { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public virtual ICollection<Store> Stores { get; set; } = new List<Store>();

    public virtual User? User { get; set; }
}
