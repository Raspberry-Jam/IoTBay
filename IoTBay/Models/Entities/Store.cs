namespace IoTBay.Models.Entities;

public partial class Store
{
    public int StoreId { get; set; }

    public int ContactId { get; set; }

    public int AddressId { get; set; }

    public virtual Address Address { get; set; } = null!;

    public virtual Contact Contact { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<StoreProductStock> StoreProductStocks { get; set; } = new List<StoreProductStock>();

    public virtual ICollection<Catalogue> Catalogues { get; set; } = new List<Catalogue>();
}
