using IoTBay.Models;
using IoTBay.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using IoTBay.Models.Views;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IoTBay.Controllers
{
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;
        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Order
        public async Task<IActionResult> Index(string searchOrderNumber = null, string searchDate = null)
        {
            var userId = GetCurrentUserId();
            var isAnonymous = userId == null;
            IQueryable<Order> orders = _context.Orders.Include(o => o.OrderProducts).ThenInclude(op => op.Product).Include(o => o.User);
            if (!isAnonymous)
            {
                orders = orders.Where(o => o.UserId == userId);
            }
            else
            {
                orders = orders.Where(o => false); // Anonymous users can't see order history
            }
            if (!string.IsNullOrEmpty(searchOrderNumber) && int.TryParse(searchOrderNumber, out int orderId))
            {
                orders = orders.Where(o => o.OrderId == orderId);
            }
            if (!string.IsNullOrEmpty(searchDate) && DateTime.TryParse(searchDate, out DateTime date))
            {
                orders = orders.Where(o => o.OrderProducts.Any() && o.OrderProducts.First().Order.OrderId == o.OrderId && o.OrderProducts.First().Order.OrderId == o.OrderId && o.OrderProducts.First().Order.OrderId == o.OrderId); // Placeholder for date logic
            }
            var orderList = await orders.ToListAsync();
            return View(orderList);
        }

        // GET: /Order/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Orders.Include(o => o.OrderProducts).ThenInclude(op => op.Product).FirstOrDefaultAsync(o => o.OrderId == id);
            if (order == null) return NotFound();
            return View(order);
        }

        // GET: /Order/Create
        public IActionResult Create()
        {
            var viewModel = new OrderViewModel
            {
                UserId = 0,
                OrderDate = DateOnly.FromDateTime(DateTime.Now)
                // Dropdowns are now hardcoded in the view model
            };
            ViewBag.Products = _context.Products.ToList();
            var latestOrder = _context.Orders.OrderByDescending(o => o.OrderId).FirstOrDefault();
            ViewBag.LatestOrderId = latestOrder?.OrderId;
            return View(viewModel);
        }

        // POST: /Order/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(OrderViewModel model, string actionType, int[] productIds, int[] amounts)
        {
            if (!ModelState.IsValid)
            {
                model.PaymentMethodOptions = new List<SelectListItem>
                {
                    new SelectListItem { Value = "CreditCard", Text = "Credit Card" },
                    new SelectListItem { Value = "BankTransfer", Text = "Bank Transfer" },
                    new SelectListItem { Value = "PayPal", Text = "PayPal" }
                };
                model.ShipmentMethodOptions = new List<SelectListItem>
                {
                    new SelectListItem { Value = "Standard", Text = "Standard" },
                    new SelectListItem { Value = "Express", Text = "Express" },
                    new SelectListItem { Value = "Pickup", Text = "Pickup" }
                };
                ViewBag.Products = _context.Products.ToList();
                return View(model);
            }

            int userId = model.UserId > 0 ? model.UserId : 1;

            // Map selected shipment method string to the correct ShipmentMethod for the user
            var shipmentMethod = _context.ShipmentMethods
                .FirstOrDefault(sm => sm.UserId == userId && sm.Method == model.ShipmentMethodId);
            if (shipmentMethod == null)
            {
                ModelState.AddModelError("ShipmentMethodId", "Selected shipment method does not exist for this user.");
                model.PaymentMethodOptions = new List<SelectListItem>
                {
                    new SelectListItem { Value = "CreditCard", Text = "Credit Card" },
                    new SelectListItem { Value = "BankTransfer", Text = "Bank Transfer" },
                    new SelectListItem { Value = "PayPal", Text = "PayPal" }
                };
                model.ShipmentMethodOptions = new List<SelectListItem>
                {
                    new SelectListItem { Value = "Standard", Text = "Standard" },
                    new SelectListItem { Value = "Express", Text = "Express" },
                    new SelectListItem { Value = "Pickup", Text = "Pickup" }
                };
                ViewBag.Products = _context.Products.ToList();
                return View(model);
            }

            // Map selected payment method string to the correct PaymentMethod for the user
            var paymentMethod = _context.PaymentMethods
                .FirstOrDefault(pm => pm.UserId == userId); // You can add a type check if you have a type field
            if (paymentMethod == null)
            {
                ModelState.AddModelError("PaymentMethodId", "Selected payment method does not exist for this user.");
                model.PaymentMethodOptions = new List<SelectListItem>
                {
                    new SelectListItem { Value = "CreditCard", Text = "Credit Card" },
                    new SelectListItem { Value = "BankTransfer", Text = "Bank Transfer" },
                    new SelectListItem { Value = "PayPal", Text = "PayPal" }
                };
                model.ShipmentMethodOptions = new List<SelectListItem>
                {
                    new SelectListItem { Value = "Standard", Text = "Standard" },
                    new SelectListItem { Value = "Express", Text = "Express" },
                    new SelectListItem { Value = "Pickup", Text = "Pickup" }
                };
                ViewBag.Products = _context.Products.ToList();
                return View(model);
            }

            var order = new Order
            {
                UserId = userId,
                PaymentMethodId = paymentMethod.PaymentMethodId,
                ShipmentMethodId = shipmentMethod.ShipmentMethodId,
                OrderDate = model.OrderDate,
                SentDate = model.SentDate
            };

            _context.Orders.Add(order);
            _context.SaveChanges();

            for (int i = 0; i < productIds.Length; i++)
            {
                if (amounts[i] > 0)
                {
                    var orderProduct = new OrderProduct
                    {
                        OrderId = order.OrderId,
                        ProductId = productIds[i],
                        Quantity = amounts[i]
                    };
                    _context.OrderProducts.Add(orderProduct);
                }
            }

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: /Order/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var order = await _context.Orders.Include(o => o.OrderProducts).FirstOrDefaultAsync(o => o.OrderId == id);
            if (order == null) return NotFound();
            ViewBag.Products = _context.Products.ToList();
            return View(order);
        }

        // POST: /Order/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Order order, List<int> productIds, List<int> amounts)
        {
            if (id != order.OrderId) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Entry(order).State = EntityState.Modified;
                // Remove old products and add new
                var oldProducts = _context.OrderProducts.Where(op => op.OrderId == id);
                _context.OrderProducts.RemoveRange(oldProducts);
                for (int i = 0; i < productIds.Count; i++)
                {
                    _context.OrderProducts.Add(new OrderProduct { OrderId = id, ProductId = productIds[i], Quantity = amounts[i] });
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Products = _context.Products.ToList();
            return View(order);
        }

        // POST: /Order/Submit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();
            // Mark as submitted (add a status property if needed)
            // order.Status = "Submitted";
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: /Order/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private int? GetCurrentUserId()
        {
            // Implement user authentication logic here
            // Return null for anonymous users
            return null;
        }
    }
} 