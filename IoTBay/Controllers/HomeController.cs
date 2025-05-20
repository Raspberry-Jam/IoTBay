using System.Diagnostics;
using IoTBay.Models;
using IoTBay.Models.Views;
using Microsoft.AspNetCore.Mvc;

namespace IoTBay.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AppDbContext _db;

    public HomeController(ILogger<HomeController> logger, AppDbContext db)
    {
        _logger = logger;
        _db = db;
    }

    public IActionResult Index()
    {
        var data = _db.Addresses.ToList();
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
    
    [HttpPost]
    public IActionResult IncreaseCounter(TestViewModel model)
    {
        model.Counter++;
        model.Addresses = _db.Addresses.ToList();
        return View("Index", model);
    }
}