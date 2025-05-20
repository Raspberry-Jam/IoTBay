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
        
        builder.WebHost.UseUrls("http://0.0.0.0:8080");

        
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
            var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
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
                ctx.Products.Add(new Product
                {
                    Name = "Keyboard",
                    Type = "mouse",
                    Price = 32.05,
                    Stock = 17,
                    ShortDescription = "Standard QWERTY layout USB Keyboard",
                    FullDescription = "This Keyboard is a part of IotBay's Standard Office Equipment Range and has full USB capability, is compatible with Windows, MacOS and Linux"
                });
                ctx.Products.Add(new Product
                {
                    Name = "Mikhail Himself",
                    Type = "keyboard",
                    Price = 69.69,
                    ShortDescription = "Balls",
                    Stock = 38,
                    FullDescription = "ghj Sackthman"
                });
                ctx.Products.Add(new Product
                {
                    Name = "asd Himself",
                    Type = "keyboard",
                    Price = 22.69,
                    ShortDescription = "Balls",
                    Stock = 8,
                    FullDescription = "Ballz Sackthman"
                });
                ctx.Products.Add(new Product
                {
                    Name = "das Himself",
                    Type = "keyboard",
                    Price = 13.69,
                    ShortDescription = "ghjghj",
                    Stock = 551,
                    FullDescription = "ghjghjghj j"
                });
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