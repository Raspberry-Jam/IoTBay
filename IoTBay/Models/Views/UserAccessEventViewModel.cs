using IoTBay.Models.Entities;

namespace IoTBay.Models.Views;

public class UserAccessEventViewModel
{
    public DateTime EventTime { get; set; }
    public AccessEventType EventType { get; set; }
}