using System.ComponentModel.DataAnnotations;
using IoTBay.Models;
using IoTBay.Models.DTOs;
using IoTBay.Models.Entities;
using IoTBay.Models.Views;
using IoTBay.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace IoTBay.Controllers;

public class CatalogueController(ILogger<CatalogueController> logger, AppDbContext db) : Controller
{
    public IActionResult Index(string? searchQuery, string? selectedCategory, decimal? minPrice, decimal? maxPrice)
{
    // Get all products from the database (no filtering yet)
    var allProducts = db.Products.Where(p => p.ProductId > 0).OrderBy(p => p.ProductId);

    // Get distinct categories for the dropdown
    var categories = allProducts.Select(p => p.Type).Distinct().ToList();
    var categorySelectList = categories.Select(c => new SelectListItem
    {
        Value = c,
        Text = c
    }).ToList();

    // Create view model for displaying categories
    var viewModel = new CatalogueFilterViewModel
    {
        SearchQuery = searchQuery,
        SelectedCategory = selectedCategory,
        MinPrice = minPrice,
        MaxPrice = maxPrice,
        ProductCategories = categorySelectList
    };

    // Perform manual validation
    var validationContext = new ValidationContext(viewModel);
    var validationResults = new List<ValidationResult>();
    var isValid = Validator.TryValidateObject(viewModel, validationContext, validationResults, true);

    // If invalid, just return the normal unfiltered page with validation errors
    if (!isValid)
    {
        foreach (var validationResult in validationResults)
        {
            foreach (var memberName in validationResult.MemberNames)
            {
                ModelState.AddModelError(memberName, validationResult.ErrorMessage!);
            }
        }

        // Return view without filtering the products (i.e., return the normal Index)
        viewModel.Products = new List<ProductEditModel>();
        return View(viewModel);  // Just show the normal page, no filtering
    }

    // Apply filters if valid
    var query = allProducts.Where(p =>
        (string.IsNullOrEmpty(searchQuery) || p.Name.ToLower().Contains(searchQuery.ToLower())) &&
        (string.IsNullOrEmpty(selectedCategory) || p.Type == selectedCategory) &&
        (!minPrice.HasValue || p.Price >= (double?)minPrice) &&
        (!maxPrice.HasValue || p.Price <= (double?)maxPrice)
    );

    var filteredProducts = query.ToList();

    // Map filtered products to ProductEditModel
    viewModel.Products = filteredProducts.Select(p => new ProductEditModel
    {
        ProductId = p.ProductId,
        Name = p.Name,
        ShortDescription = p.ShortDescription!,
        Price = p.Price,
        Stock = p.Stock,
        Type = p.Type
    }).ToList();

    // Return filtered results
    return View(viewModel);
}

    public IActionResult ProductPage(int id)
    {
        var sessionUser = SessionUtils.GetObjectFromJson<UserSessionDto>(HttpContext.Session, "currentUser");
        var query =
            from currentProduct in db.Products
            where currentProduct.ProductId == id
            select currentProduct;

        var model = new ProductPageViewModel
        {
            Product = query.SingleOrDefault()!,
            UserRole = sessionUser?.Role ?? Role.Anonymous
        };

        return View(model);
    }

    // GET: /Catalogue/ProductAdd
    [HttpGet]
    public IActionResult ProductAdd()
    {
        var categories = db.Products
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
            ProductCategories = categories,
            ImageFile = null!
        };

        return View(model);
    }

// POST: /Catalogue/ProductAdd
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ProductAdd(ProductAddModel model, string action)
    {
        if (action == "clear")
        {
            // Return an empty model with the category list reset
            var emptyModel = new ProductAddModel
            {
                ProductCategories = db.Products
                    .Select(c => new SelectListItem
                    {
                        Value = c.Type,
                        Text = c.Type
                    })
                    .Distinct()
                    .ToList(),
                Name = null!,
                Type = null!,
                Price = 0,
                ShortDescription = null!,
                ImageFile = null!
            };

            ModelState.Clear(); // Clear any previous errors or data

            return View(emptyModel);
        }

        // Default behavior: handle actual product submission
        if (!ModelState.IsValid)
        {
            model.ProductCategories = db.Products
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

        db.Products.Add(product);
        db.SaveChanges();
        
        if (model.ImageFile.Length > 0)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Photos");
            Directory.CreateDirectory(uploadsFolder); // Ensure folder exists

            var fileExtension = Path.GetExtension(model.ImageFile.FileName); // e.g., .jpg or .png
            var fileName = $"{product.ProductId}{fileExtension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                model.ImageFile.CopyTo(stream);
            }
        }

        return RedirectToAction("Index");
    }

    [HttpGet]
    [AuthenticationFilter(Role.Staff)]
    public IActionResult ProductEditDelete(int id)
    {
        var product = db.Products.FirstOrDefault(p => p.ProductId == id);

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
    [AuthenticationFilter(Role.Staff)]
    public IActionResult ProductEditDelete(ProductEditModel model)
    {
        if (!ModelState.IsValid)
        {
            // No need to repopulate ProductCategories anymore
            return View(model);
        }

        var product = db.Products.FirstOrDefault(p => p.ProductId == model.ProductId);
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

        db.SaveChanges();

        return RedirectToAction("Index");
    }

    [HttpPost]
    [AuthenticationFilter(Role.Staff)]
    [ValidateAntiForgeryToken]
    public IActionResult ProductDelete(int id)
    {
        var product = db.Products.FirstOrDefault(p => p.ProductId == id);

        if (product == null)
        {
            return NotFound();
        }

        db.Products.Remove(product);
        db.SaveChanges();

        return RedirectToAction("Index", "Catalogue");
    }
}