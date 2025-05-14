using IoTBay.Models;
using Microsoft.AspNetCore.Mvc;

namespace IoTBay.Controllers;

public class CatalogueController : Controller
{
    private readonly ILogger<CatalogueController> _logger;
    private readonly AppDbContext _db;

    public CatalogueController(ILogger<CatalogueController> logger, AppDbContext db)
    {
        _logger = logger;
        _db = db;
    }
    
    public IActionResult Index()
    {
        var products = _db.Products.ToList(); // load products
        return View(products);
    }
}