using System;
using System.Collections.Generic;

namespace IoTBay.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string Name { get; set; } = null!;

    public double? Price { get; set; }

    public string? ShortDescription { get; set; }

    public string? FullDescription { get; set; }

    public string? ThumbnailUri { get; set; }

    public string? GalleryFolderUri { get; set; }

    public virtual ICollection<CartProduct> CartProducts { get; set; } = new List<CartProduct>();

    public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();

    public virtual ICollection<StoreProductStock> StoreProductStocks { get; set; } = new List<StoreProductStock>();

    public virtual ICollection<Catalogue> Catalogues { get; set; } = new List<Catalogue>();
}