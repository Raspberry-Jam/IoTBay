using System;
using System.Collections.Generic;

namespace IoTBay.Models.Entities;

public partial class Contact
{
    public int ContactId { get; set; }

    public string GivenName { get; set; } = null!;

    public string? Surname { get; set; }

    public string Email { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public virtual User? User { get; set; }
}
