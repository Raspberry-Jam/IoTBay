using System.ComponentModel.DataAnnotations;
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

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (MaxPrice.HasValue && MaxPrice < 0)
        {
            // Return a validation message for the Price field
            yield return new ValidationResult("cannot have negative Maximum Price", new[] { nameof(MaxPrice) });
        }
        if (MinPrice.HasValue && MinPrice < 0)
        {
            // Return a validation message for the Price field
            yield return new ValidationResult("Cannot have negative MinPrice", new[] { nameof(MinPrice) });
        }
        
        if (MinPrice.HasValue && MaxPrice.HasValue && MinPrice > MaxPrice)
        {
            // Return a validation message for the Price field
            yield return new ValidationResult("Cannot have a Minimum price greater than the Maximum Price or a Maximum Price Smaller than the Minimum", new[] { nameof(MinPrice) });
        }
        
    }
    

}
