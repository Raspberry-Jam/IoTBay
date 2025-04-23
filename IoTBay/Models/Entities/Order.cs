namespace IoTBay.Models.Entities;

public partial class Order
{
    public int OrderId { get; set; }

    public int UserId { get; set; }

    public int? StoreId { get; set; }

    public int? AddressId { get; set; }

    public virtual Address? Address { get; set; }

    public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();

    public virtual Store? Store { get; set; }

    public virtual User User { get; set; } = null!;
}
