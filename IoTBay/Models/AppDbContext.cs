using IoTBay.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace IoTBay.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<CartProduct> CartProducts { get; set; }

    public virtual DbSet<Catalogue> Catalogues { get; set; }

    public virtual DbSet<Contact> Contacts { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderProduct> OrderProducts { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Staff> Staff { get; set; }

    public virtual DbSet<Store> Stores { get; set; }

    public virtual DbSet<StoreProductStock> StoreProductStocks { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
            optionsBuilder.UseNpgsql(connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresEnum("permission_enum", new[] { "clerk", "manager", "admin" })
            .HasPostgresEnum("state_enum", new[] { "act", "nsw", "nt", "qld", "sa", "tas", "vic", "wa" });

        modelBuilder
            .HasPostgresEnum<Permission>()
            .HasPostgresEnum<State>();

        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.AddressId).HasName("addresses_pkey");

            entity.ToTable("addresses");

            entity.Property(e => e.AddressId).HasColumnName("address_id");
            entity.Property(e => e.Postcode)
                .HasMaxLength(4)
                .IsFixedLength()
                .HasColumnName("postcode");
            entity.Property(e => e.StreetLine1)
                .HasMaxLength(128)
                .HasColumnName("street_line_1");
            entity.Property(e => e.StreetLine2)
                .HasMaxLength(128)
                .HasColumnName("street_line_2");
            entity.Property(e => e.Suburb)
                .HasMaxLength(128)
                .HasColumnName("suburb");
            entity.Property(e => e.State)
                .HasConversion(
                    // Disable dereference possible nullable warning because ReSharper
                    // is too stupid to see ternary statements apparently
                    #pragma warning disable CS8602
                    s => s == null ? null : $"{s.ToString().ToLower()}::state_enum", // Enforce PostgreSQL type check
                    s => s == null ? null : Enum.Parse<State>(s.Replace("::state_enum", ""), true));
                    #pragma warning enable CS8602
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.CartId).HasName("carts_pkey");

            entity.ToTable("carts");

            entity.HasIndex(e => e.UserId, "carts_user_id_key").IsUnique();

            entity.Property(e => e.CartId).HasColumnName("cart_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithOne(p => p.Cart)
                .HasForeignKey<Cart>(d => d.UserId)
                .HasConstraintName("carts_user_id_fkey");
        });

        modelBuilder.Entity<CartProduct>(entity =>
        {
            entity.HasKey(e => new { e.CartId, e.ProductId }).HasName("cart_products_pkey");

            entity.ToTable("cart_products");

            entity.Property(e => e.CartId).HasColumnName("cart_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Amount).HasColumnName("amount");

            entity.HasOne(d => d.Cart).WithMany(p => p.CartProducts)
                .HasForeignKey(d => d.CartId)
                .HasConstraintName("cart_products_cart_id_fkey");

            entity.HasOne(d => d.Product).WithMany(p => p.CartProducts)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("cart_products_product_id_fkey");
        });

        modelBuilder.Entity<Catalogue>(entity =>
        {
            entity.HasKey(e => e.CatalogueId).HasName("catalogues_pkey");

            entity.ToTable("catalogues");

            entity.Property(e => e.CatalogueId).HasColumnName("catalogue_id");

            entity.HasMany(d => d.Products).WithMany(p => p.Catalogues)
                .UsingEntity<Dictionary<string, object>>(
                    "CatalogueProductList",
                    r => r.HasOne<Product>().WithMany()
                        .HasForeignKey("ProductId")
                        .HasConstraintName("catalogue_product_list_product_id_fkey"),
                    l => l.HasOne<Catalogue>().WithMany()
                        .HasForeignKey("CatalogueId")
                        .HasConstraintName("catalogue_product_list_catalogue_id_fkey"),
                    j =>
                    {
                        j.HasKey("CatalogueId", "ProductId").HasName("catalogue_product_list_pkey");
                        j.ToTable("catalogue_product_list");
                        j.IndexerProperty<int>("CatalogueId").HasColumnName("catalogue_id");
                        j.IndexerProperty<int>("ProductId").HasColumnName("product_id");
                    });
        });

        modelBuilder.Entity<Contact>(entity =>
        {
            entity.HasKey(e => e.ContactId).HasName("contacts_pkey");

            entity.ToTable("contacts");

            entity.Property(e => e.ContactId).HasColumnName("contact_id");
            entity.Property(e => e.Email)
                .HasMaxLength(128)
                .HasColumnName("email");
            entity.Property(e => e.GivenName)
                .HasMaxLength(128)
                .HasColumnName("given_name");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(10)
                .HasColumnName("phone_number");
            entity.Property(e => e.Surname)
                .HasMaxLength(128)
                .HasColumnName("surname");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("orders_pkey");

            entity.ToTable("orders");

            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.AddressId).HasColumnName("address_id");
            entity.Property(e => e.StoreId).HasColumnName("store_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Address).WithMany(p => p.Orders)
                .HasForeignKey(d => d.AddressId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("orders_address_id_fkey");

            entity.HasOne(d => d.Store).WithMany(p => p.Orders)
                .HasForeignKey(d => d.StoreId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("orders_store_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("orders_user_id_fkey");
        });

        modelBuilder.Entity<OrderProduct>(entity =>
        {
            entity.HasKey(e => new { e.OrderId, e.ProductId }).HasName("order_products_pkey");

            entity.ToTable("order_products");

            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Amount).HasColumnName("amount");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderProducts)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("order_products_order_id_fkey");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderProducts)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("order_products_product_id_fkey");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("products_pkey");

            entity.ToTable("products");

            entity.HasIndex(e => e.Name, "products_name_key").IsUnique();

            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.FullDescription).HasColumnName("full_description");
            entity.Property(e => e.GalleryFolderUri)
                .HasMaxLength(128)
                .HasColumnName("gallery_folder_uri");
            entity.Property(e => e.Name)
                .HasMaxLength(256)
                .HasColumnName("name");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.ShortDescription)
                .HasMaxLength(512)
                .HasColumnName("short_description");
            entity.Property(e => e.ThumbnailUri)
                .HasMaxLength(128)
                .HasColumnName("thumbnail_uri");
        });

        modelBuilder.Entity<Staff>(entity =>
        {
            entity.HasKey(e => e.StaffId).HasName("staff_pkey");

            entity.ToTable("staff");

            entity.HasIndex(e => e.UserId, "staff_user_id_key").IsUnique();

            entity.Property(e => e.StaffId).HasColumnName("staff_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.Property(e => e.Permission)
                .HasConversion(
                    p => $"{p.ToString().ToLower()}::permission_enum", // Enforce PostgreSQL type check
                    p => Enum.Parse<Permission>(p.Replace("::permission_enum", ""), true));

            entity.HasOne(d => d.User).WithOne(p => p.Staff)
                .HasForeignKey<Staff>(d => d.UserId)
                .HasConstraintName("staff_user_id_fkey");
        });

        modelBuilder.Entity<Store>(entity =>
        {
            entity.HasKey(e => e.StoreId).HasName("stores_pkey");

            entity.ToTable("stores");

            entity.Property(e => e.StoreId).HasColumnName("store_id");
            entity.Property(e => e.AddressId).HasColumnName("address_id");
            entity.Property(e => e.ContactId).HasColumnName("contact_id");

            entity.HasOne(d => d.Address).WithMany(p => p.Stores)
                .HasForeignKey(d => d.AddressId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("stores_address_id_fkey");

            entity.HasOne(d => d.Contact).WithMany(p => p.Stores)
                .HasForeignKey(d => d.ContactId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("stores_contact_id_fkey");

            entity.HasMany(d => d.Catalogues).WithMany(p => p.Stores)
                .UsingEntity<Dictionary<string, object>>(
                    "StoreCatalogue",
                    r => r.HasOne<Catalogue>().WithMany()
                        .HasForeignKey("CatalogueId")
                        .HasConstraintName("store_catalogues_catalogue_id_fkey"),
                    l => l.HasOne<Store>().WithMany()
                        .HasForeignKey("StoreId")
                        .HasConstraintName("store_catalogues_store_id_fkey"),
                    j =>
                    {
                        j.HasKey("StoreId", "CatalogueId").HasName("store_catalogues_pkey");
                        j.ToTable("store_catalogues");
                        j.IndexerProperty<int>("StoreId").HasColumnName("store_id");
                        j.IndexerProperty<int>("CatalogueId").HasColumnName("catalogue_id");
                    });
        });

        modelBuilder.Entity<StoreProductStock>(entity =>
        {
            entity.HasKey(e => new { e.StoreId, e.ProductId }).HasName("store_product_stock_pkey");

            entity.ToTable("store_product_stock");

            entity.Property(e => e.StoreId).HasColumnName("store_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Stock).HasColumnName("stock");

            entity.HasOne(d => d.Product).WithMany(p => p.StoreProductStocks)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("store_product_stock_product_id_fkey");

            entity.HasOne(d => d.Store).WithMany(p => p.StoreProductStocks)
                .HasForeignKey(d => d.StoreId)
                .HasConstraintName("store_product_stock_store_id_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.ContactId, "users_contact_id_key").IsUnique();

            entity.HasIndex(e => e.Username, "users_username_key").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.AddressId).HasColumnName("address_id");
            entity.Property(e => e.ContactId).HasColumnName("contact_id");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(256)
                .HasColumnName("password_hash");
            entity.Property(e => e.PasswordSalt)
                .HasMaxLength(128)
                .HasColumnName("password_salt");
            entity.Property(e => e.Username)
                .HasMaxLength(64)
                .HasColumnName("username");

            entity.HasOne(d => d.Address).WithMany(p => p.Users)
                .HasForeignKey(d => d.AddressId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("users_address_id_fkey");

            entity.HasOne(d => d.Contact).WithOne(p => p.User)
                .HasForeignKey<User>(d => d.ContactId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("users_contact_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
