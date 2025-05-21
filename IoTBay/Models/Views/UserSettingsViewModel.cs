using System.ComponentModel.DataAnnotations;
using IoTBay.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IoTBay.Models.Views;

public class UserSettingsViewModel : IValidatableObject
{
    [DataType(DataType.Password)]
    public string? OldPassword { get; set; }
    
    [DataType(DataType.Password)]
    public string? NewPassword { get; set; }
    
    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
    public string? ConfirmPassword { get; set; }
    
    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; }
    
    [DataType(DataType.Text)]
    public string? GivenName { get; set; }
    
    [DataType(DataType.Text)]
    public string? Surname { get; set; }
    
    [DataType(DataType.PhoneNumber)]
    public string? PhoneNumber { get; set; }
    
    public IEnumerable<UserAccessEventViewModel>? AccessEvents { get; set; }
    
    public int UserId { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if ((NewPassword == null) != (ConfirmPassword == null)) // If something is set for one but not the other
        {
            yield return new ValidationResult("Please confirm your password.");
        }

        if (PhoneNumber == null) yield break;
        if (PhoneNumber.Length < 10)
        {
            yield return new ValidationResult("Phone number must be at least 10 characters.");
        }
    }
}