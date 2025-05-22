using IoTBay.Models;
using IoTBay.Models.DTOs;
using IoTBay.Models.Entities;
using IoTBay.Models.Views;
using IoTBay.Repositories;
using IoTBay.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IoTBay.Controllers;

public class ShipmentController(
    AppDbContext db,
    ShipmentMethodRepository shipmentMethodRepository,
    ILogger<ShipmentController> logger) : Controller
{
    // List all shipments for the current user
    [AuthenticationFilter]
    public IActionResult Index(ShipmentMethodIndexViewModel? model)
    {
        var sessionDto = SessionUtils.GetObjectFromJson<UserSessionDto>(HttpContext.Session, "currentUser");
        if (sessionDto == null) // If they managed to execute this action without session data, something went wrong
        {
            logger.LogError("Settings page authenticated with an invalid session");
            return RedirectToAction("Error", "Home");
        }

        var shipments = db.ShipmentMethods
            .Include(s => s.Address)
            .AsEnumerable()
            .Where(sm => sm.UserId == sessionDto.UserId)
            .Where(sm =>
                sm.Method.Contains(model?.MethodSearch ??
                                   "")) // Filter by contains method if the MethodSearch string isn't null
            .Where(sm =>
                sm.Method.Contains(model?.AddressSearch ??
                                   "")); // Filter by contains address if the AddressSearch string isn't null

        var viewModel = new ShipmentMethodIndexViewModel
        {
            ShipmentMethods = shipments
        };
        return View(viewModel);
    }

    // Show form to create new shipment
    [AuthenticationFilter]
    [HttpGet]
    public IActionResult Create()
    {
        var sessionDto = SessionUtils.GetObjectFromJson<UserSessionDto>(HttpContext.Session, "currentUser");
        if (sessionDto == null) // If they managed to execute this action without session data, something went wrong
        {
            logger.LogError("Settings page authenticated with an invalid session");
            return RedirectToAction("Error", "Home");
        }

        var model = new ShipmentMethodCreateViewModel
        {
            UserId = sessionDto.UserId
        };
        return View(model);
    }

    // Process new shipment creation
    [AuthenticationFilter]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ShipmentMethodCreateViewModel model)
    {
        var sessionDto = SessionUtils.GetObjectFromJson<UserSessionDto>(HttpContext.Session, "currentUser");
        if (sessionDto == null) // If they managed to execute this action without session data, something went wrong
        {
            logger.LogError("Settings page authenticated with an invalid session");
            return RedirectToAction("Error", "Home");
        }

        if (ModelState.IsValid)
        {
            // TODO: Address Validation
            var shipment = new ShipmentMethod
            {
                UserId = sessionDto.UserId,
                Address = model.Address,
                Method = model.Method
            };

            db.ShipmentMethods.Add(shipment);
            await db.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        return View(model);
    }

    // Show shipment edit form
    [AuthenticationFilter]
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var sessionDto = SessionUtils.GetObjectFromJson<UserSessionDto>(HttpContext.Session, "currentUser");
        if (sessionDto == null) // If they managed to execute this action without session data, something went wrong
        {
            logger.LogError("Settings page authenticated with an invalid session");
            return RedirectToAction("Error", "Home");
        }

        var shipment = await shipmentMethodRepository.GetById(id);

        if (shipment.UserId != sessionDto.UserId) // User is somehow accessing a ShipmentMethod they do not own
        {
            return RedirectToAction("AccessDenied", "Home");
        }

        var model = new ShipmentMethodEditViewModel
        {
            ShipmentMethod = shipment
        };

        return View(model);
    }

    // Process shipment update
    [AuthenticationFilter]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ShipmentMethodEditViewModel model)
    {
        var sessionDto = SessionUtils.GetObjectFromJson<UserSessionDto>(HttpContext.Session, "currentUser");
        if (sessionDto == null) // If they managed to execute this action without session data, something went wrong
        {
            logger.LogError("Settings page authenticated with an invalid session");
            return RedirectToAction("Error", "Home");
        }

        shipmentMethodRepository.Update(model.ShipmentMethod);
        await shipmentMethodRepository.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    [AuthenticationFilter]
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var sessionDto = SessionUtils.GetObjectFromJson<UserSessionDto>(HttpContext.Session, "currentUser");
        if (sessionDto == null) // If they managed to execute this action without session data, something went wrong
        {
            logger.LogError("Settings page authenticated with an invalid session");
            return RedirectToAction("Error", "Home");
        }

        var shipment = await shipmentMethodRepository.GetById(id);
        shipment = await shipmentMethodRepository.LoadWithAddress(shipment); // Eager load the address navigation property

        if (shipment.UserId != sessionDto.UserId)
        {
            return RedirectToAction("AccessDenied", "Home");
        }

        return View(shipment);
    }

    // Process shipment deletion
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(ShipmentMethod shipmentMethod)
    {
        var sessionDto = SessionUtils.GetObjectFromJson<UserSessionDto>(HttpContext.Session, "currentUser");
        if (sessionDto == null) // If they managed to execute this action without session data, something went wrong
        {
            logger.LogError("Settings page authenticated with an invalid session");
            return RedirectToAction("Error", "Home");
        }

        if (shipmentMethod.UserId != sessionDto.UserId) // User is somehow accessing a ShipmentMethod they do not own
        {
            return RedirectToAction("AccessDenied", "Home");
        }

        shipmentMethodRepository.Delete(shipmentMethod);
        await db.SaveChangesAsync();

        return RedirectToAction("Index");
    }
}