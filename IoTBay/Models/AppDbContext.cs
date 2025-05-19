using IoTBay.DataAccess;
using IoTBay.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace IoTBay.Models;

public partial class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<Contact> Contacts { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderProduct> OrderProducts { get; set; }

    public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ShipmentMethod> ShipmentMethods { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserAccessEvent> UserAccessEvents { get; set; }

    public virtual DbSet<UserCartProduct> UserCartProducts { get; set; }

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
                    s => s == null ? null : s.ToString().ToLower(), // Enforce PostgreSQL type check
                    s => s == null ? null : Enum.Parse<State>(s, true))
                    #pragma warning restore CS8602
                .HasColumnName("state");
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
                .IsFixedLength()
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
            entity.Property(e => e.OrderDate).HasColumnName("order_date");
            entity.Property(e => e.PaymentMethodId).HasColumnName("payment_method_id");
            entity.Property(e => e.SentDate).HasColumnName("sent_date");
            entity.Property(e => e.ShipmentMethodId).HasColumnName("shipment_method_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.PaymentMethod).WithMany(p => p.Orders)
                .HasForeignKey(d => d.PaymentMethodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("orders_payment_method_id_fkey");

            entity.HasOne(d => d.ShipmentMethod).WithMany(p => p.Orders)
                .HasForeignKey(d => d.ShipmentMethodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("orders_shipment_method_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("orders_user_id_fkey");
        });

        modelBuilder.Entity<OrderProduct>(entity =>
        {
            entity.HasKey(e => new { e.OrderId, e.ProductId }).HasName("order_products_pkey");

            entity.ToTable("order_products");

            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderProducts)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_products_order_id_fkey");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderProducts)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_products_product_id_fkey");
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasKey(e => e.PaymentMethodId).HasName("payment_methods_pkey");

            entity.ToTable("payment_methods");

            entity.Property(e => e.PaymentMethodId).HasColumnName("payment_method_id");
            entity.Property(e => e.CardNumber)
                .HasMaxLength(16)
                .IsFixedLength()
                .HasColumnName("card_number");
            entity.Property(e => e.Cvv)
                .HasMaxLength(3)
                .IsFixedLength()
                .HasColumnName("cvv");
            entity.Property(e => e.Expiry).HasColumnName("expiry");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.PaymentMethods)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("payment_methods_user_id_fkey");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("products_pkey");

            entity.ToTable("products");

            entity.HasIndex(e => e.Name, "products_name_key").IsUnique();

            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.FullDescription).HasColumnName("full_description");
            entity.Property(e => e.Name)
                .HasMaxLength(256)
                .HasColumnName("name");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.ShortDescription)
                .HasMaxLength(512)
                .HasColumnName("short_description");
            entity.Property(e => e.Stock).HasColumnName("stock");
            entity.Property(e => e.Type)
                .HasMaxLength(128)
                .HasColumnName("type");
        });

        modelBuilder.Entity<ShipmentMethod>(entity =>
        {
            entity.HasKey(e => e.ShipmentMethodId).HasName("shipment_methods_pkey");

            entity.ToTable("shipment_methods");

            entity.Property(e => e.ShipmentMethodId).HasColumnName("shipment_method_id");
            entity.Property(e => e.AddressId).HasColumnName("address_id");
            entity.Property(e => e.Method)
                .HasMaxLength(128)
                .HasColumnName("method");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Address).WithMany(p => p.ShipmentMethods)
                .HasForeignKey(d => d.AddressId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("shipment_methods_address_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.ShipmentMethods)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("shipment_methods_user_id_fkey");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.SupplierId).HasName("suppliers_pkey");

            entity.ToTable("suppliers");

            entity.Property(e => e.SupplierId).HasColumnName("supplier_id");
            entity.Property(e => e.AddressId).HasColumnName("address_id");
            entity.Property(e => e.CompanyName)
                .HasMaxLength(128)
                .HasColumnName("company_name");
            entity.Property(e => e.ContactId).HasColumnName("contact_id");

            entity.HasOne(d => d.Address).WithMany(p => p.Suppliers)
                .HasForeignKey(d => d.AddressId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("suppliers_address_id_fkey");

            entity.HasOne(d => d.Contact).WithMany(p => p.Suppliers)
                .HasForeignKey(d => d.ContactId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("suppliers_contact_id_fkey");

            entity.HasMany(d => d.Products).WithMany(p => p.Suppliers)
                .UsingEntity<Dictionary<string, object>>(
                    "SupplierProduct",
                    r => r.HasOne<Product>().WithMany()
                        .HasForeignKey("ProductId")
                        .HasConstraintName("supplier_products_product_id_fkey"),
                    l => l.HasOne<Supplier>().WithMany()
                        .HasForeignKey("SupplierId")
                        .HasConstraintName("supplier_products_supplier_id_fkey"),
                    j =>
                    {
                        j.HasKey("SupplierId", "ProductId").HasName("supplier_products_pkey");
                        j.ToTable("supplier_products");
                        j.IndexerProperty<int>("SupplierId").HasColumnName("supplier_id");
                        j.IndexerProperty<int>("ProductId").HasColumnName("product_id");
                    });
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.ContactId, "users_contact_id_key").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.ContactId).HasColumnName("contact_id");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(256)
                .IsFixedLength()
                .HasColumnName("password_hash");
            entity.Property(e => e.PasswordSalt)
                .HasMaxLength(128)
                .IsFixedLength()
                .HasColumnName("password_salt");
            entity.Property(e => e.Role)
                .HasConversion(
                    r => r.ToString(), 
                    r => Enum.Parse<Role>(r, true))
                .HasColumnName("role");

            entity.HasOne(d => d.Contact).WithOne(p => p.User)
                .HasForeignKey<User>(d => d.ContactId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("users_contact_id_fkey");
        });

        modelBuilder.Entity<UserAccessEvent>(entity =>
        {
            entity.HasKey(e => e.UserAccessEventId).HasName("user_access_events_pkey");

            entity.ToTable("user_access_events");

            entity.Property(e => e.UserAccessEventId).HasColumnName("user_access_event_id");
            entity.Property(e => e.EventTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("event_time");
            entity.Property(e => e.EventType)
                .HasMaxLength(8)
                .HasColumnName("event_type");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.UserAccessEvents)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("user_access_events_user_id_fkey");
        });

        modelBuilder.Entity<UserCartProduct>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.ProductId }).HasName("user_cart_products_pkey");

            entity.ToTable("user_cart_products");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Product).WithMany(p => p.UserCartProducts)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_cart_products_product_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.UserCartProducts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_cart_products_user_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
