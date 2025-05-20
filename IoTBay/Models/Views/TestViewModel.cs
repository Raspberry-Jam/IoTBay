using IoTBay.Models.Entities;

namespace IoTBay.Models.Views;

public class TestViewModel
{
    public int Counter { get; set; }
    public required List<Address> Addresses { get; set; }
}