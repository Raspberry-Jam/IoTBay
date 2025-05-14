using IoTBay.Models;
using Microsoft.AspNetCore.Mvc;

namespace IoTBay.Controllers;

public class ProductPageController : Controller
{
    private readonly ILogger<ProductPageController> _logger;
    private readonly AppDbContext _db;

    public ProductPageController(ILogger<ProductPageController> logger, AppDbContext db)
    {
        _logger = logger;
        _db = db;
    }
    
    public IActionResult ProductPage(int id)
    {
        var query =
            from currentProduct in _db.Products
            where currentProduct.ProductId == id
            select currentProduct;
        
        return View(query.SingleOrDefault());
    }
}