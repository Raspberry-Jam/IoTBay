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
                var products = new List<Product>
                    {
                    new Product {
                        Name = "Standard USB Keyboard",
                        Type = "Keyboard",
                        Price = 32.05,
                        Stock = 50,
                        ShortDescription = "Standard QWERTY layout USB Keyboard",
                        FullDescription = "This Keyboard is part of IotBay's Standard Office Equipment Range, featuring USB connectivity and compatibility with Windows, MacOS, and Linux."
                    },
                    new Product {
                        Name = "Mechanical RGB Keyboard",
                        Type = "Keyboard",
                        Price = 89.99,
                        Stock = 50,
                        ShortDescription = "RGB Mechanical Gaming Keyboard with Blue Switches",
                        FullDescription = "Perfect for gamers, with tactile mechanical switches, anti-ghosting keys, and customizable lighting."
                    },
                    new Product {
                        Name = "Wireless Bluetooth Keyboard",
                        Type = "Keyboard",
                        Price = 45.50,
                        Stock = 50,
                        ShortDescription = "Slim wireless Bluetooth keyboard",
                        FullDescription = "Compatible with iOS, Android, and Windows, designed for portability and extended battery life."
                    },
                    new Product {
                        Name = "Ergonomic Split Keyboard",
                        Type = "Keyboard",
                        Price = 74.95,
                        Stock = 50,
                        ShortDescription = "Ergonomic split design keyboard with wrist rest",
                        FullDescription = "Reduces wrist strain with a natural typing layout and soft wrist support."
                    },
                    new Product {
                        Name = "Compact 60% Keyboard",
                        Type = "Keyboard",
                        Price = 58.00,
                        Stock = 50,
                        ShortDescription = "Portable 60% mechanical keyboard",
                        FullDescription = "A minimalist layout with mechanical keys and USB-C connection for portability and speed."
                    },
                    new Product {
                        Name = "Wireless Optical Mouse",
                        Type = "Mouse",
                        Price = 25.99,
                        Stock = 50,
                        ShortDescription = "2.4GHz wireless mouse with ergonomic design",
                        FullDescription = "Comfortable and precise, includes a USB nano receiver and DPI adjustment."
                    },
                    new Product {
                        Name = "Gaming Mouse with RGB",
                        Type = "Mouse",
                        Price = 49.99,
                        Stock = 50,
                        ShortDescription = "High-precision gaming mouse with RGB lighting",
                        FullDescription = "Designed for gamers with adjustable DPI, programmable buttons, and custom RGB effects."
                    },
                    new Product {
                        Name = "4K UHD Monitor 27\"",
                        Type = "Monitor",
                        Price = 299.99,
                        Stock = 50,
                        ShortDescription = "27-inch Ultra HD display with HDMI and DisplayPort",
                        FullDescription = "Perfect for work and entertainment with vibrant colors, fast response, and ultra-high resolution."
                    },
                    new Product {
                        Name = "1080p Office Monitor 22\"",
                        Type = "Monitor",
                        Price = 119.99,
                        Stock = 100,
                        ShortDescription = "Full HD LED monitor ideal for productivity",
                        FullDescription = "Reliable and affordable monitor for everyday tasks and clear visuals."
                    },
                    new Product {
                        Name = "USB-C Hub 7-in-1",
                        Type = "Peripheral",
                        Price = 34.95,
                        Stock = 100,
                        ShortDescription = "Expand your laptop’s connectivity with USB-C hub",
                        FullDescription = "Features HDMI, USB 3.0, SD card reader, and more — essential for modern laptops."
                    },
                    new Product {
                        Name = "Noise-Cancelling Headset",
                        Type = "Headset",
                        Price = 79.99,
                        Stock = 100,
                        ShortDescription = "Over-ear noise-canceling headset with mic",
                        FullDescription = "Ideal for calls, gaming, and music — immersive sound with comfortable fit."
                    },
                    new Product {
                        Name = "USB Conference Microphone",
                        Type = "Microphone",
                        Price = 54.90,
                        Stock = 100,
                        ShortDescription = "Omnidirectional microphone for meetings",
                        FullDescription = "Plug-and-play USB mic with noise reduction, ideal for remote work and Zoom calls."
                    },
                    new Product {
                        Name = "Webcam 1080p HD",
                        Type = "Webcam",
                        Price = 39.99,
                        Stock = 100,
                        ShortDescription = "HD webcam with built-in microphone",
                        FullDescription = "Delivers clear video and audio for streaming, video calls, and online meetings."
                    },
                    new Product {
                        Name = "Portable External SSD 1TB",
                        Type = "Storage",
                        Price = 119.99,
                        Stock = 100,
                        ShortDescription = "Fast USB-C SSD with 1TB capacity",
                        FullDescription = "Lightweight and durable external drive with high-speed data transfer."
                    },
                    new Product {
                        Name = "Laser Printer Mono",
                        Type = "Printer",
                        Price = 149.00,
                        Stock = 100,
                        ShortDescription = "High-speed monochrome laser printer",
                        FullDescription = "Efficient and low-cost printing for office environments, supports wireless printing."
                    },
                    new Product {
                        Name = "Wireless Charging Pad",
                        Type = "Accessory",
                        Price = 29.95,
                        Stock = 100,
                        ShortDescription = "Qi-certified wireless charger for smartphones",
                        FullDescription = "Supports fast wireless charging for most major devices with a sleek, compact design."
                    },
                    new Product {
                        Name = "Mechanical Tenkeyless Keyboard",
                        Type = "Keyboard",
                        Price = 69.99,
                        Stock = 100,
                        ShortDescription = "Compact TKL keyboard with brown switches",
                        FullDescription = "Offers mechanical precision without the number pad — great for desk space and portability."
                    },
                    new Product {
                        Name = "Keyboard Cleaning Kit",
                        Type = "Accessory",
                        Price = 14.99,
                        Stock = 100,
                        ShortDescription = "Multi-tool set for cleaning mechanical keyboards",
                        FullDescription = "Includes keycap puller, brush, and cleaning solution for maintaining keyboard performance."
                    },
                    new Product {
                        Name = "Foldable Travel Keyboard",
                        Type = "Keyboard",
                        Price = 39.95,
                        Stock = 100,
                        ShortDescription = "Portable foldable Bluetooth keyboard",
                        FullDescription = "Compact and foldable for easy transport, compatible with smartphones and tablets."
                    },
                    new Product {
                        Name = "Smart LED Desk Lamp",
                        Type = "Lighting",
                        Price = 44.99,
                        Stock = 100,
                        ShortDescription = "LED lamp with touch control and USB charging",
                        FullDescription = "Modern desk lamp with adjustable brightness and built-in USB port for device charging."
                    }
                };
                ctx.Products.AddRange(products);
                
                var product = new Product
                {
                    Name = "Keyboard4Order",
                    Type = "Keyboard",
                    Price = 32.05,
                    Stock = 25,
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