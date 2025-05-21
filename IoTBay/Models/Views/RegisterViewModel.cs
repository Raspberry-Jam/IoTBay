using System.ComponentModel.DataAnnotations;
using IoTBay.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IoTBay.Models.Views;

/// <summary>
/// A validatable view model for the registration page that checks for password match.
/// </summary>
public class RegisterViewModel
{
    // This is dangerous. I am disabling this because I know the members should be non-null by the time they are accessed.
#pragma warning disable CS8618
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passwords do not match.")] // Validate password matching
    public string ConfirmPassword { get; set; }

    // Create a list of states stored within the State enum, and a default "Select State" option that returns an empty (null) value.
    public IEnumerable<SelectListItem> States =>
        new[] { new SelectListItem { Text = "--- Select State (Optional) ---", Value = "" } }
            .Concat(
                Enum.GetValues<State>()
                    .Select(s => new SelectListItem
                    {
                        Text = s.ToString(),
                        Value = s.ToString()
                    }));
    public Contact Contact { get; set; }
}