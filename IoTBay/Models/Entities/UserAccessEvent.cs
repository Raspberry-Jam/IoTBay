using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace IoTBay.Models.Entities;

public enum AccessEventType
{
    Login, Logout, Unknown
}

public partial class UserAccessEvent
{
    public int UserAccessEventId { get; }

    public int UserId { get; set; }

    public DateTime EventTime { get; set; }

    [Column("event_type")]
    public AccessEventType EventType { get; set; }

    public virtual User User { get; set; } = null!;
}
