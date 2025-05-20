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

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            if (!ctx.Addresses.Any())
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
                ctx.Products.Add(new Product
                {
                    Name = "Keyboard",
                    Price = 32.05,
                    ShortDescription = "Standard QWERTY layout USB Keyboard",
                    FullDescription = "This Keyboard is a part of IotBay's Standard Office Equipment Range and has full USB capability, is compatible with Windows, MacOS and Linux"
                });
                ctx.Products.Add(new Product
                {
                    Name = "Mikhail Himself",
                    Price = 69.69,
                    ShortDescription = "Balls",
                    FullDescription = "ghj Sackthman"
                });
                ctx.Products.Add(new Product
                {
                    Name = "asd Himself",
                    Price = 22.69,
                    ShortDescription = "Balls",
                    FullDescription = "Ballz Sackthman"
                });
                ctx.Products.Add(new Product
                {
                    Name = "das Himself",
                    Price = 13.69,
                    ShortDescription = "ghjghj",
                    FullDescription = "ghjghjghj j"
                });
                ctx.SaveChanges();
            }
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