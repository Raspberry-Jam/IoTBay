using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IoTBay.Models.Views;

public class ProductAddViewModel : IValidatableObject
{
    
    [Required]
    public required string Name { get; set; }

    [Required]
    [Range(0.01, double.MaxValue,ErrorMessage = "Price must be Larger than 0.")]
    public required double Price { get; set; }

    [Required]
    public string? FullDescription { get; set; }

    [Required]
    public required string Category { get; set; }  // This will hold the selected category (e.g. name or ID)

    [Required]
    public required string ShortDescription { get; set; }

    // Dropdown source
    public List<SelectListItem> ProductCategories { get; set; } = new(); 

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // Validate Price for maximum 2 decimal places
        if (Math.Abs(Math.Round(Price, 2) - Price) > 0.000001)
        {
            // Return a validation message for the Price field
            yield return new ValidationResult("Price musasdasdasdal places.", new[] { nameof(Price) });
        }

        // Add other custom validation logic as needed...
    }

}