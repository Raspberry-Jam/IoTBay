using System.ComponentModel.DataAnnotations;
using IoTBay.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IoTBay.Models.Views;

public class RegisterViewModel : IValidatableObject
{
    // This is dangerous. I am disabling this because I know the members should be non-null by the time they are accessed.
    #pragma warning disable CS8618
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; }

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
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Contact.PhoneNumber != null && Contact.PhoneNumber.Length != 10)
        {
            yield return new ValidationResult("Phone number must be 10 digits", ["Contact.Email"]);
        }
    }
}