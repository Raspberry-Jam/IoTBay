namespace IoTBay.Models.Views;

public class UserOrderViewModel
{
    public int OrderId { get; set; }
    public DateOnly OrderDate { get; set; }
    public DateOnly? SentDate { get; set; }
    public double TotalPrice { get; set; }
}