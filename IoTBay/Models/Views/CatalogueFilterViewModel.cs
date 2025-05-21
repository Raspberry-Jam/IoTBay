using Microsoft.AspNetCore.Mvc.Rendering;

namespace IoTBay.Models.Views;

public class CatalogueFilterViewModel
{
    // Filter inputs
    public string? SearchQuery { get; set; }
    public string? SelectedCategory { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }

    // Dropdown source
    public List<SelectListItem> ProductCategories { get; set; } = new();

    // The products to display
    public List<ProductEditModel> Products { get; set; } = new();
    
}