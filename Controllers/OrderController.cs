using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Order order, List<int> productIds, List<int> amounts)
        {
            if (ModelState.IsValid)
            {
                // Handle anonymous user creation if needed
                if (order.UserId == 0)
                {
                    var anonUser = new User { Username = "anon_" + Guid.NewGuid(), PasswordHash = "", PasswordSalt = "", Contact = new Contact(), Orders = new List<Order>() };
                    _context.Users.Add(anonUser);
                    await _context.SaveChangesAsync();
                    order.UserId = anonUser.UserId;
                }
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                for (int i = 0; i < productIds.Count; i++)
                {
                    _context.OrderProducts.Add(new OrderProduct { OrderId = order.OrderId, ProductId = productIds[i], Amount = amounts[i] });
                }
                await _context.SaveChangesAsync();
                // Return Ok for AJAX
                return Ok();
            }
            ViewBag.Products = _context.Products.ToList();
            return View(order);
        }
    }
} 