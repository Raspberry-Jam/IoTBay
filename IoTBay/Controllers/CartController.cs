using System.Diagnostics;
using IoTBay.Models;
using IoTBay.Models.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace IoTBay.Controllers;

public class CartController(AppDbContext db) : Controller
{

    // Action to display the cart
    public IActionResult Index()
    {
        // Get the cart from session
        var cartJson = HttpContext.Session.GetString("Cart");

        var cartViewModel =
            // If the cart does not exist in the session, create a new one
            string.IsNullOrEmpty(cartJson) ? new CartViewModel() :
            // Deserialize the cart data from session
            JsonConvert.DeserializeObject<CartViewModel>(cartJson)!;

        // Optionally, recalculate the total price
        cartViewModel.CalculateTotalPrice();

        // Return the cart view model to the view
        return View(cartViewModel);
    }

    // Action to add items to the cart
    [HttpPost]
    public async Task<IActionResult> Index(int productId, int quantity)
    {
        // Fetch the product from the database
        var product = await db.Products.FirstOrDefaultAsync(p => p.ProductId == productId);

        if (product == null)
        {
            // Redirect or handle error if product not found
            return RedirectToAction("Index", "Catalogue");
        }

        // Create a simplified cart item
        var cartItem = new CartItemViewModel
        {
            ProductId = product.ProductId,
            ProductName = product.Name,
            Quantity = quantity
        };

        // Get the existing cart from session
        var cartJson = HttpContext.Session.GetString("Cart");
        var cartViewModel = (string.IsNullOrEmpty(cartJson)
            ? new CartViewModel()
            : JsonConvert.DeserializeObject<CartViewModel>(cartJson)) ?? throw new InvalidOperationException();

        // Add or update item
        var existingItem = cartViewModel.Items.FirstOrDefault(i => i.ProductId == productId);
        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            cartViewModel.Items.Add(cartItem);
        }

        // Serialize and store updated cart in session
        var updatedCartJson = JsonConvert.SerializeObject(cartViewModel);
        HttpContext.Session.SetString("Cart", updatedCartJson);

        return RedirectToAction("Index");
    }

    public IActionResult RemoveFromCart(int productId)
    {
        // Retrieve the cart from session
        var cartJson = HttpContext.Session.GetString("Cart");
        var cartViewModel = string.IsNullOrEmpty(cartJson)
            ? new CartViewModel()
            : JsonConvert.DeserializeObject<CartViewModel>(cartJson);

        // Find and remove the item
        Debug.Assert(cartViewModel != null, nameof(cartViewModel) + " != null");
        var itemToRemove = cartViewModel.Items.FirstOrDefault(i => i.ProductId == productId);
        if (itemToRemove != null)
        {
            cartViewModel.Items.Remove(itemToRemove);
        }

        // Store updated cart back in session
        var updatedCartJson = JsonConvert.SerializeObject(cartViewModel);
        HttpContext.Session.SetString("Cart", updatedCartJson);

        return RedirectToAction("Index");
    }


    // Optional: Clear the cart completely
    public IActionResult ClearCart()
    {
        HttpContext.Session.Remove("Cart");  // Removes the cart from session
        return RedirectToAction("Index");
    }
}