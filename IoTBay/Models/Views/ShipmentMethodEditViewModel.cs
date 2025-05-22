using IoTBay.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IoTBay.Models.Views;

public class ShipmentMethodEditViewModel
{
    public ShipmentMethod ShipmentMethod { get; set; }
    public IEnumerable<SelectListItem> States =>
        new[] { new SelectListItem { Text = "--- Select State (Optional) ---", Value = "" } }
            .Concat(
                Enum.GetValues<State>()
                    .Select(s => new SelectListItem
                    {
                        Text = s.ToString(),
                        Value = s.ToString()
                    }));
}