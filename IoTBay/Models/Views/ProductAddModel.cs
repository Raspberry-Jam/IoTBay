using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IoTBay.Models.Views;

public class ProductAddModel : IValidatableObject
{
    
    [Required]
    public required string Name { get; set; }
    
    [Required]
    public required string Type { get; set; }  // This will hold the selected category (e.g. name or ID)

    [Required]
    [Range(0.01, double.MaxValue,ErrorMessage = "Price must be Larger than 0.")]
    public required double Price { get; set; }
    
    [Required]
    
    public int Stock { get; set; }
    
    [Required]
    public required string ShortDescription { get; set; }

    [Required]
    public string? FullDescription { get; set; }
    
    [Display(Name = "Upload Product Image")]
    public required IFormFile ImageFile { get; set; }
    

    
    // Dropdown source
    public List<SelectListItem> ProductCategories { get; set; } = new(); 

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (double.Round((double)Price!, 2) == Price)
        {
            yield return new ValidationResult("Price 2 d places.", new[] { nameof(Price) });
        }
        
        if (Stock < 0)
        {
            yield return new ValidationResult("Cannot have negative Stock", new[] { nameof(Stock) });
        }
    }

}