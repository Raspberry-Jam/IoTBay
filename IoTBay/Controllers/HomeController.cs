using System.Diagnostics;
using IoTBay.Models;
using IoTBay.Models.Views;
using Microsoft.AspNetCore.Mvc;

namespace IoTBay.Controllers;

/// <summary>
/// Handle business logic for the Home view collection
/// </summary>
/// <param name="logger">Dependency Injection: Application logger</param>
/// <param name="db">Dependency Injection: Database context</param>
public class HomeController(ILogger<HomeController> logger, AppDbContext db) : Controller
{
    /// <summary>
    /// Set up the view model for the Index page, and then send it to the Home/Index.cshtml View for rendering.
    /// </summary>
    /// <returns>Index View</returns>
    public IActionResult Index()
    {
        var data = db.Addresses.ToList();
        var counterModel = new TestViewModel { Counter = 0, Addresses = data };
        return View(counterModel);
    }

    /// <summary>
    /// Send the Privacy View page.
    /// </summary>
    /// <returns>Privacy View</returns>
    public IActionResult Privacy() => View();

    /// <summary>
    /// Set up the error view model and send along the Error View
    /// </summary>
    /// <returns>Error View</returns>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)] // Disable caching for this page
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    /// <summary>
    /// Send the AccessDenied View page.
    /// </summary>
    /// <returns>AccessDenied View</returns>
    public IActionResult AccessDenied() => View();
    
    /// <summary>
    /// A demo action to show server-side state tracking and model updating.
    /// </summary>
    /// <param name="model">Index View with updated model</param>
    /// <returns></returns>
    [HttpPost]
    public IActionResult IncreaseCounter(TestViewModel model)
    {
        model.Counter++;
        return View("Index", model);
    }
}