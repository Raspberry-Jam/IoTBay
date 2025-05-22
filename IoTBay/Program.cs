using IoTBay.DataAccess;
using IoTBay.DataAccess.Implementations;
using IoTBay.DataAccess.Interfaces;
using IoTBay.Models;
using IoTBay.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace IoTBay;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        // Add services to the container.
        builder.Services.AddControllersWithViews();
        builder.Logging.AddConsole();
        builder.Logging.SetMinimumLevel(LogLevel.Information);
        builder.Services.AddDbContext<AppDbContext>(options => options
                .UseNpgsql(Environment.GetEnvironmentVariable("CONNECTION_STRING"))
                .EnableSensitiveDataLogging());
        
        // Register model entity repositories for dependency injection
        builder.Services.AddScoped<IAppDbContext, AppDbContext>();
        builder.Services.AddScoped<IAddressRepository, AddressRepository>();
        builder.Services.AddScoped<IContactRepository, ContactRepository>();
        builder.Services.AddScoped<IOrderRepository, OrderRepository>();
        builder.Services.AddScoped<IPaymentMethod, PaymentMethodRepository>();
        builder.Services.AddScoped<IProductRepository, ProductRepository>();
        builder.Services.AddScoped<IShipmentMethodRepository, ShipmentMethodRepository>();
        builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();

        var app = builder.Build();

       using (var scope = app.Services.CreateScope())
        {
            var ctx = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
            var addressQuery = from a in ctx.Addresses
                where a.AddressId >= 1
                select a;
            var productQuery = from p in ctx.Products
                where p.ProductId >= 1
                select p;
            var userQuery = from u in ctx.Users
                where u.UserId >= 1
                select u;

            if (!userQuery.Any())
            {
                var contact = new Contact
                {
                    Email = "test@test.com",
                    GivenName = "Tester",
                    Surname = "Debugger"
                };
                ctx.Contacts.Add(contact);
                var passwordHash = Utils.HashUtils.HashPassword("password", out var salt);
                ctx.Users.Add(new User
                {
                    PasswordHash = passwordHash,
                    PasswordSalt = salt,
                    Contact = contact
                });
            }
            
            if (!addressQuery.Any())
            {
                var address1 = new Address { StreetLine1 = "123 Test Street", Suburb = "Springfield", State = State.ACT, Postcode = "1234" };
                var address2 = new Address { StreetLine1 = "245 Bug Blvd", StreetLine2 = "Unit 87", Suburb = "Woodland", State = State.NSW, Postcode = "8823" };
                ctx.Addresses.Add(address1);
                ctx.Addresses.Add(address2);
                var contact1 = new Contact { Email = "user1@example.com", GivenName = "User", Surname = "One" };
                var contact2 = new Contact { Email = "user2@example.com", GivenName = "User", Surname = "Two" };
                var contact3 = new Contact { Email = "user3@example.com", GivenName = "User", Surname = "Three" };
                ctx.Contacts.Add(contact1);
                ctx.Contacts.Add(contact2);
                ctx.Contacts.Add(contact3);
                ctx.SaveChanges();
                
                var user1 = new User { Role = Role.Customer, PasswordHash = "password", PasswordSalt = "salt", ContactId = contact1.ContactId };
                var user2 = new User { Role = Role.Customer, PasswordHash = "password", PasswordSalt = "salt", ContactId = contact2.ContactId };
                var user3 = new User { Role = Role.Customer, PasswordHash = "password", PasswordSalt = "salt", ContactId = contact3.ContactId };
                ctx.Users.Add(user1);
                ctx.Users.Add(user2);
                ctx.Users.Add(user3);
                ctx.SaveChanges();
                
                var address1FromDb = ctx.Addresses.First();
                var address2FromDb = ctx.Addresses.Skip(1).First();
                var user1FromDb = ctx.Users.First();
                var user2FromDb = ctx.Users.Skip(1).First();
                var user3FromDb = ctx.Users.Skip(2).First();

                ctx.ShipmentMethods.Add(new ShipmentMethod
                {
                    UserId = user1FromDb.UserId,
                    AddressId = address1FromDb.AddressId,
                    Method = "Standard"
                });
                ctx.PaymentMethods.Add(new PaymentMethod
                {
                    UserId = user1FromDb.UserId,
                    CardNumber = "1234123412341234",
                    Cvv = "123",
                    Expiry = new DateOnly(2027, 5, 22)
                });
                
                ctx.ShipmentMethods.Add(new ShipmentMethod
                {
                    UserId = user2FromDb.UserId,
                    AddressId = address2FromDb.AddressId,
                    Method = "Express"
                });
                ctx.PaymentMethods.Add(new PaymentMethod
                {
                    UserId = user2FromDb.UserId,
                    CardNumber = "4321432143214321",
                    Cvv = "321",
                    Expiry = new DateOnly(2028, 6, 15)
                });
                
                ctx.ShipmentMethods.Add(new ShipmentMethod
                {
                    UserId = user3FromDb.UserId,
                    AddressId = address1FromDb.AddressId,
                    Method = "Pickup"
                });
                ctx.PaymentMethods.Add(new PaymentMethod
                {
                    UserId = user3FromDb.UserId,
                    CardNumber = "5678567856785678",
                    Cvv = "456",
                    Expiry = new DateOnly(2029, 7, 10)
                });
                
                ctx.SaveChanges();
            } 
            else
            {
                Console.WriteLine("Addresses already exist");
            }

            if (!productQuery.Any()) {
                ctx.Products.Add(new Product
                {
                    Name = "Keyboard",
                    Type = "Keyboard",
                    Price = 32.05,
                    ShortDescription = "Standard QWERTY layout USB Keyboard",
                    FullDescription = "This Keyboard is a part of IotBay's Standard Office Equipment Range and has full USB capability, is compatible with Windows, MacOS and Linux"
                });
                
            }
            else
            {
                Console.WriteLine("Products already exist");
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

        app.UseAuthorization();

        app.MapStaticAssets();
        app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();

        app.Run();
    }
}