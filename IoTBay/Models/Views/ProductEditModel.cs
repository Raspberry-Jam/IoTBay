using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IoTBay.Models.Views;

public class ProductEditModel : IValidatableObject
{
    [Required] public required int ProductId { get; init; }

    [Required] public required string Name { get; init; }

    [Required] public required string Type { get; init; } // Category is "Type" in our DB

    [Required] public required double? Price { get; init; }

    [Required] public int Stock { get; init; }

    [Required] public required string ShortDescription { get; init; }

    [Required] public string? FullDescription { get; init; }
    public List<SelectListItem> ProductCategories { get; init; } = [];
    // List of Categories

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // Validate Price for maximum 2 decimal places
        if (Math.Abs((double)(Math.Round((double)Price!, 2) - Price)) > 0.000001)
            {
                // Return a validation message for the Price field
                yield return new ValidationResult("Price must be correct to 2 decimal places", new[] { nameof(Price) });
            }
        if (Price <= 0)
        {
            // Return a validation message for the Price field
            yield return new ValidationResult("Price must not be less than or equal to 0", new[] { nameof(Price) });
        }

        if (Stock < 0)
        {
            // Return a validation message for the Stock field
            yield return new ValidationResult("Cannot have negative Stock", new[] { nameof(Stock) });
        }
        
    }
}
