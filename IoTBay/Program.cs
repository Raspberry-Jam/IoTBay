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
            if (!addressQuery.Any())
            {
                ctx.Addresses.Add(new Address
                {
                    StreetLine1 = "123 Test Street",
                    Suburb = "Springfield",
                    State = State.ACT,
                    Postcode = "1234"
                });
                ctx.Addresses.Add(new Address
                {
                    StreetLine1 = "245 Bug Blvd",
                    StreetLine2 = "Unit 87",
                    Suburb = "Woodland",
                    State = State.NSW,
                    Postcode = "8823"
                });
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