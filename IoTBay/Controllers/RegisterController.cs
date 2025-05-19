using IoTBay.DataAccess.Interfaces;
using IoTBay.Models;
using IoTBay.Models.Entities;
using IoTBay.Models.Views;
using Microsoft.AspNetCore.Mvc;

namespace IoTBay.Controllers;

public class RegisterController : Controller
{
    private readonly ILogger<RegisterController> _logger;
    private readonly IUserRepository _userRepository;

    public RegisterController(ILogger<RegisterController> logger, IUserRepository userRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
    }

    public IActionResult Index()
    {
        var model = new RegisterViewModel();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                _logger.LogError(error.ErrorMessage);
            }
            // TODO: Add model error handling with ModelState.AddModelError()
            _logger.LogError("Register was passed with an invalid model");
            return RedirectToAction("Index");
        }

        if (model.ProceedWithoutAddress) // The user has acknowledged the partial address, and agrees to proceed
        {
            model.Address = null;
        }

        if (model.Address != null)
        {
            bool isAnyFieldFilled = !string.IsNullOrWhiteSpace(model.Address.StreetLine1) ||
                                    !string.IsNullOrWhiteSpace(model.Address.Suburb) ||
                                    !string.IsNullOrWhiteSpace(model.Address.State.ToString()) ||
                                    !string.IsNullOrWhiteSpace(model.Address.Postcode);
            bool isAnyFieldEmpty = string.IsNullOrWhiteSpace(model.Address.StreetLine1) ||
                                   string.IsNullOrWhiteSpace(model.Address.Suburb) ||
                                   string.IsNullOrWhiteSpace(model.Address.State.ToString()) ||
                                   string.IsNullOrWhiteSpace(model.Address.Postcode);

            if (isAnyFieldEmpty && isAnyFieldFilled && !model.ProceedWithoutAddress) // This indicates a partial address
            {
                ModelState.AddModelError("partialAddress",
                    "Your address is incomplete. Click 'Proceed Without Address' to ignore it.");
                return View("Index", model);
            }
        }
        
        // TODO: We will be using emails for login only, so remove username check and usernames entirely
        // Check if username is in use
        var testEmail = await _userRepository.GetByEmail(model.Contact.Email);

        if (testEmail != null) 
            ModelState.AddModelError("emailInUse", "This email is already in use! Please use another one.");

        string passwordHash = Utils.HashUtils.HashPassword(model.Password, out var salt);

        User user = new User
        {
            PasswordHash = passwordHash,
            PasswordSalt = salt,
            Contact = model.Contact,
        };

        await _userRepository.Add(user);
        
        return View("Welcome", user);
    }
}