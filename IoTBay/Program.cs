using IoTBay.Models;
using IoTBay.Models.Entities;
using IoTBay.Repositories;
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

        using (var scope = app.Services.CreateScope())
        {
            var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
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