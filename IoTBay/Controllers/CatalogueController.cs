using IoTBay.Models;
using IoTBay.Models.Entities;
using IoTBay.Models.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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
    
    public IActionResult ProductPage(int id)
    {
        var query =
            from currentProduct in _db.Products
            where currentProduct.ProductId == id
            select currentProduct;
        
        return View(query.SingleOrDefault());
    }

    // GET: /Catalogue/ProductAdd
    [HttpGet]
    public IActionResult ProductAdd()
    {
        var categories = _db.Products
            .Select(c => new SelectListItem
            {
                Value = c.Name, // Use Category instead of Name for better UX
                Text = c.Name
            })
            .Distinct()
            .ToList();

        var model = new ProductAddViewModel
        {
            Name = null!,
            Price = 0,
            FullDescription = null,
            Category = null!,
            ShortDescription = null!,
            ProductCategories = categories
        };

        return View(model);
    }

// POST: /Catalogue/ProductAdd
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ProductAdd(ProductAddViewModel model)
    {
        if (!ModelState.IsValid)
        {
            // Repopulate categories in case validation fails
            model.ProductCategories = _db.Products
                .Select(c => new SelectListItem
                {
                    Value = c.Name,
                    Text = c.Name
                })
                .Distinct()
                .ToList();

            return View(model);
        }
        
        model.Price = Math.Round(model.Price, 2, MidpointRounding.AwayFromZero);
        
        var product = new Product
        {
            Name = model.Name,
            Price = model.Price,
            // Category = model.Category,
            ShortDescription = model.ShortDescription,
            FullDescription = model.FullDescription
        };

        _db.Products.Add(product);
        _db.SaveChanges();

        return RedirectToAction("Index");
    }


}