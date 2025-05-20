using System;
using System.Collections.Generic;

namespace IoTBay.Models.Entities;

public partial class UserAccessEvent
{
    public int UserAccessEventId { get; set; }

    public int UserId { get; set; }

    public DateTime EventTime { get; set; }

    public string EventType { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
