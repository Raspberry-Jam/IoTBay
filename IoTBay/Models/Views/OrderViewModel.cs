using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IoTBay.Models.Views;

public class OrderViewModel
{
    [Required]
    public int UserId { get; set; }

    [Required]
    public string PaymentMethodId { get; set; } = string.Empty;

    [Required]
    public string ShipmentMethodId { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Date)]
    public DateOnly OrderDate { get; set; }

    [DataType(DataType.Date)]
    public DateOnly? SentDate { get; set; }

    // Dropdown options
    public IEnumerable<SelectListItem> PaymentMethodOptions { get; set; } = new List<SelectListItem>
    {
        new SelectListItem { Value = "CreditCard", Text = "Credit Card" },
        new SelectListItem { Value = "BankTransfer", Text = "Bank Transfer" },
        //new SelectListItem { Value = "PayPal", Text = "PayPal" }
    };

    public IEnumerable<SelectListItem> ShipmentMethodOptions { get; set; } = new List<SelectListItem>
    {
        new SelectListItem { Value = "Standard", Text = "Standard" },
        new SelectListItem { Value = "Express", Text = "Express" },
        new SelectListItem { Value = "Pickup", Text = "Pickup" }
    };
}