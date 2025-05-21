using System.Diagnostics;
using IoTBay.Models;
using IoTBay.Models.Views;
using Microsoft.AspNetCore.Mvc;

namespace IoTBay.Controllers;

public class HomeController(ILogger<HomeController> logger, AppDbContext db) : Controller
{
    public IActionResult Index()
    {
        var data = db.Addresses.ToList();
        var counterModel = new TestViewModel { Counter = 0, Addresses = data };
        return View(counterModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    
    public IActionResult AccessDenied() => View();
    [HttpPost]
    public IActionResult IncreaseCounter(TestViewModel model)
    {
        model.Counter++;
        model.Addresses = db.Addresses.ToList();
        return View("Index", model);
    }
}