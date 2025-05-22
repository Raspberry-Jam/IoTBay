using IoTBay.Models;
using IoTBay.Models.Entities;
using IoTBay.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IoTBay;

public class Program
{
    public static void Main(string[] args)
    {
        /*
         * NOTE: Since the current deployment of this application is ephemeral and the persistence storage is wiped
         * with each run of the containers, this causes issues with the built-in ASP.NET Core Authentication features.
         *
         * ASP.NET Core uses a system called a "Key Ring" in order to encrypt user session data on the server, and gives
         * a public key to the user, which is stored in the browser cookies. When the containers restart, this key ring
         * has been deleted and thus the private key for the user's browser cookie is lost. Since the browser still
         * holds onto this old cookie and sends it to the web application with each request, this causes the key ring
         * to warn the server admin (by printing an error in the log) that the browser cookie could not be "unprotected".
         *
         * This is not an unhandled error. This error is handled by printing it to the console, and invalidating the
         * authentication request of an outdated cookie.
         */
        
        var builder = WebApplication.CreateBuilder(args);

        builder.WebHost.UseUrls("http://0.0.0.0:8080");
        
        // Add services to the container.
        builder.Services.AddControllersWithViews();
        builder.Logging.AddConsole();
        builder.Logging.SetMinimumLevel(LogLevel.Information);
        builder.Services.AddDbContext<AppDbContext>(options => options
                .UseNpgsql(Environment.GetEnvironmentVariable("CONNECTION_STRING"))
                .EnableSensitiveDataLogging());
        
        // Enable session support
        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromDays(1);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
        
        // Register model entity repositories for dependency injection
        builder.Services.AddScoped<AppDbContext>();
        builder.Services.AddScoped<AddressRepository>();
        builder.Services.AddScoped<ContactRepository>();
        builder.Services.AddScoped<OrderRepository>();
        builder.Services.AddScoped<PaymentMethodRepository>();
        builder.Services.AddScoped<ProductRepository>();
        builder.Services.AddScoped<ShipmentMethodRepository>();
        builder.Services.AddScoped<SupplierRepository>();
        builder.Services.AddScoped<UserRepository>();

        var app = builder.Build();

        // Insert test data into the database
        using (var scope = app.Services.CreateScope())
        {
            var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var addressQuery = from a in ctx.Addresses
                where a.AddressId >= 1
                select a;
            var contactQuery = from c in ctx.Contacts
                where c.ContactId >= 1
                select c;
            var productQuery = from p in ctx.Products
                where p.ProductId >= 1
                select p;
            var userQuery = from u in ctx.Users
                where u.UserId >= 1
                select u;
            var orderQuery = from o in ctx.Orders
                where o.OrderId >= 1
                select o;
            var supplierQuery = from s in ctx.Suppliers
                where s.SupplierId >= 1
                select s;
            if (!contactQuery.Any())
            {
                var contact = new Contact
                {
                    Email = "test@test.com",
                    GivenName = "Tester",
                    Surname = "Debugger"
                };
                ctx.Contacts.Add(contact);
                var supplierContact = new Contact
                {
                    Email = "supplier@supplier.com",
                    GivenName = "John",
                    PhoneNumber = "0011223344"
                };
                if (!productQuery.Any())
                {
                    var product = new Product
                    {
                        Name = "Keyboard",
                        Type = "Keyboard",
                        Price = 32.05,
                        ShortDescription = "Standard QWERTY layout USB Keyboard",
                        FullDescription =
                            "This Keyboard is a part of IotBay's Standard Office Equipment Range and has full USB capability, is compatible with Windows, MacOS and Linux"
                    };
                    ctx.Products.Add(product);
                    if (!userQuery.Any())
                    {
                        var passwordHash = Utils.HashUtils.HashPassword("password", out var salt);
                        var user = new User
                        {
                            PasswordHash = passwordHash,
                            PasswordSalt = salt,
                            Contact = contact,
                            Role = Role.Customer
                        };
                        ctx.Users.Add(user);
                        if (!orderQuery.Any())
                        {
                            var order = new Order
                            {
                                User = user,
                                ShipmentMethodId = -1,
                                PaymentMethodId = -1,
                                OrderDate = DateOnly.FromDateTime(DateTime.Now)
                            };
                            order.OrderProducts = new List<OrderProduct>
                            {
                                new() {
                                    Order = order,
                                    Product = product,
                                    Quantity = 13
                                }
                            };
                            ctx.Orders.Add(order);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Products already exist");
                }
                if (!addressQuery.Any())
                {
                    ctx.Addresses.Add(new Address
                    {
                        StreetLine1 = "123 Test Street",
                        Suburb = "Springfield",
                        State = State.ACT,
                        Postcode = "1234"
                    });
                    var supplierAddress = new Address
                    {
                        StreetLine1 = "245 Bug Blvd",
                        StreetLine2 = "Unit 87",
                        Suburb = "Woodland",
                        State = State.NSW,
                        Postcode = "8823"
                    };
                    ctx.Addresses.Add(supplierAddress);
                    if (!supplierQuery.Any())
                    {
                        var supplier1 = new Supplier
                        {
                            Address = supplierAddress,
                            Contact = supplierContact,
                            CompanyName = "Supplier Co"
                        };
                        ctx.Suppliers.Add(supplier1);
                    }
                } 
                else
                {
                    Console.WriteLine("Addresses already exist");
                }
            }
            
            ctx.SaveChanges();
        }

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseRouting();

        app.UseSession();

        app.UseAuthorization();

        app.MapStaticAssets();
        app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();

        app.Run();
    }
}