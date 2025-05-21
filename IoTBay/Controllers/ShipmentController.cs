using IoTBay.Models;
using IoTBay.Models.DTOs;
using IoTBay.Models.Entities;
using IoTBay.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IoTBay.Controllers;

public class ShipmentController(AppDbContext db) : Controller
{
    private readonly AppDbContext _db = db;
[AuthenticationFilter]
    // List all shipments for the current user
    public async Task<IActionResult> Index(string? searchTerm = null, DateTime? searchDate = null)
    {
        var sessionDto = SessionUtils.GetObjectFromJson<UserSessionDto>(HttpContext.Session, "currentUser");

        var query = _db.Shipments
            .Include(s => s.Order)
            .Where(s => s.Order.UserId == sessionDto.UserId);

        // Apply search filters
        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(s => s.ShippingMethod.Contains(searchTerm));
        }

        if (searchDate.HasValue)
        {
            var date = DateOnly.FromDateTime(searchDate.Value);
            query = query.Where(s => s.ShippingDate == date);
        }

        var shipments = await query.ToListAsync();
        return View(shipments);
    }

    // Show form to create new shipment
    public async Task<IActionResult> Create(int orderId)
    {
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return RedirectToAction("Index", "Register");
        }

        var order = await _db.Orders.FindAsync(orderId);
        if (order == null || order.UserId != userId)
        {
            return NotFound();
        }

        var model = new Shipment { OrderId = orderId };
        return View(model);
    }

    // Process new shipment creation
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Shipment shipment)
    {
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return RedirectToAction("Index", "Register");
        }

        var order = await _db.Orders.FindAsync(shipment.OrderId);
        if (order == null || order.UserId != userId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            shipment.Status = "pending";
            _db.Shipments.Add(shipment);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(shipment);
    }

    // Show shipment edit form
    public async Task<IActionResult> Edit(int id)
    {
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return RedirectToAction("Index", "Register");
        }

        var shipment = await _db.Shipments
            .Include(s => s.Order)
            .FirstOrDefaultAsync(s => s.ShipmentId == id);

        if (shipment == null || shipment.Order.UserId != userId || shipment.Status != "pending")
        {
            return NotFound();
        }

        return View(shipment);
    }

    // Process shipment update
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Shipment shipment)
    {
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return RedirectToAction("Index", "Register");
        }

        var existingShipment = await _db.Shipments
            .Include(s => s.Order)
            .FirstOrDefaultAsync(s => s.ShipmentId == id);

        if (existingShipment == null || existingShipment.Order.UserId != userId || existingShipment.Status != "pending")
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            existingShipment.ShippingMethod = shipment.ShippingMethod;
            existingShipment.ShippingDate = shipment.ShippingDate;
            existingShipment.DeliveryAddress = shipment.DeliveryAddress;
            
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(shipment);
    }

    // Process shipment deletion
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return RedirectToAction("Index", "Register");
        }

        var shipment = await _db.Shipments
            .Include(s => s.Order)
            .FirstOrDefaultAsync(s => s.ShipmentId == id);

        if (shipment == null || shipment.Order.UserId != userId || shipment.Status != "pending")
        {
            return NotFound();
        }

        _db.Shipments.Remove(shipment);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}