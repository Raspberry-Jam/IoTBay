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

        var products =
            from product in _db.Products
            where product.ProductId > 0
            orderby product.ProductId
            select product;
            
        return View(products.ToList());
        
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
                Value = c.Type, // Use Category instead of Name for better UX
                Text = c.Type
            })
            .Distinct()
            .ToList();

        var model = new ProductAddModel
        {
            Name = null!,
            Price = 0,
            FullDescription = null,
            Stock = 0,
            Type = null!,
            ShortDescription = null!,
            ProductCategories = categories
        };

        return View(model);
    }

// POST: /Catalogue/ProductAdd
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ProductAdd(ProductAddModel model)
    {
        if (!ModelState.IsValid)
        {
            // Repopulate categories in case validation fails
            model.ProductCategories = _db.Products
                .Select(c => new SelectListItem
                {
                    Value = c.Type,
                    Text = c.Type
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
            Type = model.Type,
            Stock = model.Stock,
            ShortDescription = model.ShortDescription,
            FullDescription = model.FullDescription
        };

        _db.Products.Add(product);
        _db.SaveChanges();

        return RedirectToAction("Index");
    }


    [HttpGet]
    public IActionResult ProductEditDelete(int id)
    {
        var product = _db.Products.FirstOrDefault(p => p.ProductId == id);

        if (product == null)
        {
            return NotFound();
        }

        var model = new ProductEditModel
        {
            ProductId = product.ProductId,
            Name = product.Name,
            Price = product.Price,
            Stock = product.Stock,
            FullDescription = product.FullDescription,
            Type = product.Type,
            ShortDescription = product.ShortDescription!

        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ProductEditDelete(ProductEditModel model)
    {
        if (!ModelState.IsValid)
        {
            // No need to repopulate ProductCategories anymore
            return View(model);
        }

        var product = _db.Products.FirstOrDefault(p => p.ProductId == model.ProductId);
        if (product == null)
        {
            return NotFound();
        }

        product.Name = model.Name;
        product.Price = Math.Round((double)model.Price!, 2, MidpointRounding.AwayFromZero);
        product.Type = model.Type;
        product.Stock = model.Stock;
        product.ShortDescription = model.ShortDescription;
        product.FullDescription = model.FullDescription;

        _db.SaveChanges();

        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ProductDelete(int id)
    {
        var product = _db.Products.FirstOrDefault(p => p.ProductId == id);

        if (product == null)
        {
            return NotFound();
        }

        _db.Products.Remove(product);
        _db.SaveChanges();

        return RedirectToAction("Index", "Catalogue");
    }
}