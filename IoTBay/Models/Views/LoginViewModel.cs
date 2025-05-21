using System.ComponentModel.DataAnnotations;

namespace IoTBay.Models.Views;

/// <summary>
/// A basic view model to structure the login page with.
/// </summary>
public class LoginViewModel
{
    [Required]
    public string Email { get; set; }
    
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}