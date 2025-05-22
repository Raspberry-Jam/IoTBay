using IoTBay.Models;
using IoTBay.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IoTBay.Controllers
{
    public class SuppliersManageController : Controller
    {
        private readonly AppDbContext _db;
        private readonly ILogger<SuppliersManageController> _logger;

        public SuppliersManageController(AppDbContext db, ILogger<SuppliersManageController> logger)
        {
            _db = db;
            _logger = logger;
        }

        // GET: SuppliersManage/Index
        // Displays a list of suppliers with their associated Contact and Address details.
        public async Task<IActionResult> Index()
        {
            var suppliers = await _db.Set<Supplier>()
                                     .Include(s => s.Contact)
                                     .Include(s => s.Address)
                                     .ToListAsync();
            return View(suppliers);
        }

        // GET: SuppliersManage/Details
        // Displays detailed information for a specific supplier.
        public async Task<IActionResult> Details(int id)
        {
            var supplier = await _db.Set<Supplier>()
                                    .Include(s => s.Address)
                                    .Include(s => s.Contact)
                                    .FirstOrDefaultAsync(s => s.SupplierId == id);

            if (supplier == null)
                return NotFound();

            return View(supplier);
        }

        // GET: SuppliersManage/Create
        // Displays the form for creating a new supplier.
        public IActionResult Create()
        {
            // Initialize an empty supplier with nested objects to avoid null references.
            var supplier = new Supplier
            {
                Contact = new Contact(),
                Address = new Address()
            };
            return View(supplier);
        }

        // POST: SuppliersManage/Create
        // Handles the creation of a new supplier record.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Supplier supplier)
        {
            if (!ModelState.IsValid)
                return View(supplier);

            try
            {
                _db.Set<Supplier>().Add(supplier);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Error creating supplier with CompanyName: {CompanyName}", supplier.CompanyName);
                ModelState.AddModelError(string.Empty, "A database error occurred while saving the supplier. Please try again.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error creating supplier with CompanyName: {CompanyName}", supplier.CompanyName);
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again.");
            }
            return View(supplier);
        }

        // GET: SuppliersManage/Edit
        // Displays the form for editing an existing supplier.
        public async Task<IActionResult> Edit(int id)
        {
            var supplier = await _db.Set<Supplier>()
                                    .Include(s => s.Address)
                                    .Include(s => s.Contact)
                                    .FirstOrDefaultAsync(s => s.SupplierId == id);

            if (supplier == null)
                return NotFound();

            return View(supplier);
        }

        // POST: SuppliersManage/Edit
        // Processes updates to an existing supplier record.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Supplier supplier)
        {
            if (!ModelState.IsValid)
                return View(supplier);

            try
            {
                // Retrieve the current entity from the database.
                var existingSupplier = await _db.Set<Supplier>()
                                                .Include(s => s.Address)
                                                .Include(s => s.Contact)
                                                .FirstOrDefaultAsync(s => s.SupplierId == id);
                if (existingSupplier == null)
                    return NotFound();

                // Update the basic fields.
                existingSupplier.CompanyName = supplier.CompanyName;

                // Update contact fields.
                if (existingSupplier.Contact != null && supplier.Contact != null)
                {
                    existingSupplier.Contact.GivenName = supplier.Contact.GivenName;
                    existingSupplier.Contact.Surname = supplier.Contact.Surname;
                    existingSupplier.Contact.Email = supplier.Contact.Email;
                    existingSupplier.Contact.PhoneNumber = supplier.Contact.PhoneNumber;
                }

                // Update address fields.
                if (existingSupplier.Address != null && supplier.Address != null)
                {
                    existingSupplier.Address.StreetLine1 = supplier.Address.StreetLine1;
                    existingSupplier.Address.StreetLine2 = supplier.Address.StreetLine2;
                    existingSupplier.Address.Suburb = supplier.Address.Suburb;
                    existingSupplier.Address.State = supplier.Address.State;
                    existingSupplier.Address.Postcode = supplier.Address.Postcode;
                }

                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException dbEx)
            {
                if (!SupplierExists(supplier.SupplierId))
                    return NotFound();

                _logger.LogError(dbEx, "Concurrency error while editing supplier with ID {SupplierId}", supplier.SupplierId);
                ModelState.AddModelError(string.Empty, "A concurrency error occurred. Please reload the page and try again.");
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error while editing supplier with ID {SupplierId}", supplier.SupplierId);
                ModelState.AddModelError(string.Empty, "A database error occurred while saving changes. Please try again.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while editing supplier with ID {SupplierId}", supplier.SupplierId);
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again.");
            }
            return View(supplier);
        }
        

        // GET: SuppliersManage/Delete
        // Displays a confirmation view before deleting a supplier.
        public async Task<IActionResult> Delete(int id)
        {
            var supplier = await _db.Set<Supplier>()
                                    .Include(s => s.Address)
                                    .Include(s => s.Contact)
                                    .FirstOrDefaultAsync(s => s.SupplierId == id);
            if (supplier == null)
                return NotFound();

            return View(supplier);
        }

        // POST: SuppliersManage/Delete
        // Processes the deletion of the supplier after confirmation.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var supplier = await _db.Set<Supplier>().FindAsync(id);
            if (supplier != null)
            {
                try
                {
                    _db.Set<Supplier>().Remove(supplier);
                    await _db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error deleting supplier with ID {SupplierId}", id);
                    ModelState.AddModelError(string.Empty, "Error deleting supplier. Please try again later.");
                    return View(supplier);
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // Private helper method to check if a supplier exists by its id.
        private bool SupplierExists(int id)
        {
            return _db.Set<Supplier>().Any(s => s.SupplierId == id);
        }
    }
}