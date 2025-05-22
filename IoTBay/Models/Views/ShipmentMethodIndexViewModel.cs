using IoTBay.Models.Entities;

namespace IoTBay.Models.Views;

public class ShipmentMethodIndexViewModel
{
    public IEnumerable<ShipmentMethod> ShipmentMethods { get; set; }
    public string? MethodSearch { get; set; }
    public string? AddressSearch { get; set; }
}