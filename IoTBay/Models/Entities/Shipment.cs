using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IoTBay.Models.Entities;

public class Shipment
{
    public int ShipmentId { get; set; }
    
    public int OrderId { get; set; }

    [Required]
    [Display(Name = "Shipping Method")]
    public string ShippingMethod { get; set; } = null!;

    [Required]
    [Display(Name = "Shipping Date")]
    public DateOnly ShippingDate { get; set; }

    [Required]
    [Display(Name = "Delivery Address")]
    public string DeliveryAddress { get; set; } = null!;

    [Display(Name = "Status")]
    public string Status { get; set; } = "pending";

    public virtual Order Order { get; set; } = null!;
}