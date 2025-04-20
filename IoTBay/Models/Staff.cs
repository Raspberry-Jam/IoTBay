using System;
using System.Collections.Generic;

namespace IoTBay.Models;

public partial class Staff
{
    public int StaffId { get; set; }

    public int UserId { get; set; }

    public virtual User User { get; set; } = null!;
}
