using IoTBay.Models.DTOs;
using IoTBay.Models.Entities;
using IoTBay.Models.Views;
using IoTBay.Repositories;
using IoTBay.Utils;
using Microsoft.AspNetCore.Mvc;

namespace IoTBay.Controllers;

public class UserController(ILogger<UserController> logger, UserRepository userRepository, OrderRepository orderRepository)
    : Controller
{
    /// <summary>
    /// A basic index view that lets the user navigate to the login or registration functions.
    /// </summary>
    /// <returns>Login View</returns>
    public IActionResult Index()
    {
        var model = new LoginViewModel();
        return View(model);
    }

    /// <summary>
    /// A bare User Registration view page that allows the user to enter their details and register as a customer.
    /// </summary>
    /// <returns>Register View</returns>
    [HttpGet]
    public IActionResult Register()
    {
        var model = new RegisterViewModel();
        return View(model);
    }

    /// <summary>
    /// Handles the registration form data, validating its format and checking if the user's email is available, and
    /// if the passwords match. Returns to the Register View with errors if details are incorrect or the email is
    /// already registered.
    /// </summary>
    /// <param name="model">Filled form data returned by the Register View</param>
    /// <returns>Welcome View if successful, or Register View if unsuccessful</returns>
    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        // Check if the model state is valid, and if not, log any errors and return a fresh model to the View
        if (!ModelState.IsValid)
        {
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                logger.LogError(error.ErrorMessage);
            }
            // TODO: Add proper model error handling
            logger.LogError("Register was passed with an invalid model");
            return View(new RegisterViewModel()); // Create a new model that is not invalid
        }
        
        // Check if a user with this email already exists
        var testEmail = await userRepository.GetByEmail(model.Contact.Email);

        if (testEmail != null) // If they do, add a model error and return to the view
        {
            ModelState.AddModelError("emailInUse", "This email is already in use! Please use another one.");
            return View(model);
        }

        // Hash the user's password, and generate a random salt
        string passwordHash = HashUtils.HashPassword(model.Password, out var salt);

        // Create a new user object with the validated email, hashed password and salt, and a default role of Customer
        User user = new User
        {
            PasswordHash = passwordHash,
            PasswordSalt = salt,
            Contact = model.Contact,
            Role = Role.Customer
        };

        // Add to and synchronise database
        await userRepository.Add(user);
        await userRepository.SaveChangesAsync();
        
        // Create a new user browser session
        var userSessionDto = new UserSessionDto
        {
            UserId = user.UserId,
            GivenName = user.Contact!.GivenName,
            Role = user.Role
        };
        
        // Send them to the Welcome View
        return View("Welcome", user);
    }

    /// <summary>
    /// Handles user login requests. Checks if a user with the requested email exists, and if the password matches
    /// the password hash stored in the database. If either fails, it sends a warning back as a response. If successful,
    /// it sets the user browser session, and continues to the Welcome View.
    /// </summary>
    /// <param name="model">Filled form data from the Index View</param>
    /// <returns>Welcome View if successful, Index View if unsuccessful</returns>
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        // Check error state of the model, and redirect to index if the model is malformed
        if (!ModelState.IsValid)
        {
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                logger.LogError(error.ErrorMessage);
            }
            // TODO: Add proper model error handling
            logger.LogError("Register was passed with an invalid model");
            return RedirectToAction("Index");
        }

        // Check that the user email exists
        var user = await userRepository.GetByEmail(model.Email);
        if (user == null)
        {
            // Send back an error message to the login page if the email does not match to any user
            ModelState.AddModelError("incorrectLogin", "Incorrect email or password. Please try again.");
            return View("Index", model);
        }

        // Hash the password using the stored database salt, and compare to the stored hash to see if it is correct
        if (!HashUtils.VerifyPassword(user.PasswordHash!, model.Password, user.PasswordSalt!))
        {
            // Send back an error message to the login page if the password is incorrect for this user
            ModelState.AddModelError("incorrectLogin","Incorrect email or password. Please try again.");
            return View("Index", model);
        }

        // Create a UserSessionDto instance to track the user's browser session
        var userSessionDto = new UserSessionDto
        {
            UserId = user.UserId,
            GivenName = user.Contact!.GivenName,
            Role = user.Role
        };
        
        // Set the session data
        SessionUtils.SetObjectAsJson(HttpContext.Session, "currentUser", userSessionDto);
        
        user.UserAccessEvents.Add(new UserAccessEvent
        {
            EventTime = DateTime.Now,
            EventType = AccessEventType.Login,
            UserId = user.UserId
        });
        
        userRepository.Update(user);
        await userRepository.SaveChangesAsync();

        // Send the user to the home page
        return RedirectToAction("Index", "Home");
    }
    
    // Allow both HttpGet and HttpPost
    /// <summary>
    /// Clear the session data of this user, logging them out. Only allow this action if the user is already logged in.
    /// </summary>
    /// <returns>Home Index View</returns>
    [AuthenticationFilter]
    public async Task<IActionResult> Logout()
    {
        var sessionDto = SessionUtils.GetObjectFromJson<UserSessionDto>(HttpContext.Session, "currentUser");
        // We aren't going to handle null errors here because the user is logging out. If there is a problem, then
        // it's going to go away when the session is cleared.
        if (sessionDto != null)
        {
            var user = await userRepository.LoadFullUser(sessionDto.UserId);
            if (user != null)
            {
                user.UserAccessEvents.Add(new UserAccessEvent // Add a logout access event to the user
                {
                    EventTime = DateTime.Now,
                    EventType = AccessEventType.Logout,
                    UserId = user.UserId
                });
                await userRepository.SaveChangesAsync();
            }
        }
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }

    /// <summary>
    /// Check browser session state to authenticate access, create a UserSettingsViewModel from the user session
    /// data, and send it to the Settings View to render.
    /// </summary>
    /// <returns>Settings View</returns>
    [HttpGet]
    [AuthenticationFilter]
    public async Task<IActionResult> Settings()
    {
        // Get the session data from the request
        var sessionDto = SessionUtils.GetObjectFromJson<UserSessionDto>(HttpContext.Session, "currentUser");
        if (sessionDto == null) // If they managed to execute this action without session data, something went wrong
        {
            logger.LogError("Settings page authenticated with an invalid session");
            return RedirectToAction("Error", "Home");
        }
        
        // Get the user corresponding to the session data
        var user = await userRepository.LoadFullUser(sessionDto.UserId);
        if (user == null) // If they have session data without a corresponding user, something went wrong
        {
            logger.LogError("Settings page authenticated with a non-existent user");
            return RedirectToAction("Error", "Home");
        }

        var accessEvents = user.UserAccessEvents
            .Select(e => new UserAccessEventViewModel
            {
                EventTime = e.EventTime,
                EventType = e.EventType
            });

        var ordersShallow = user.Orders;

        var orders = await Task.WhenAll(ordersShallow.Select(o => orderRepository.DeepLoadOrderProducts(o)));

        var userOrders = orders.Select(o =>
        {
            var sum = o.OrderProducts.Sum(op => op.Product.Price * op.Quantity) ?? 0;
            return new UserOrderViewModel
            {
                OrderId = o.OrderId,
                OrderDate = o.OrderDate,
                SentDate = o.SentDate,
                TotalPrice = sum
            };
        });
        
        // Create a settings view model and populate it with the important data
        var model = new UserSettingsViewModel
        {
            Email = user.Contact!.Email,
            GivenName = user.Contact!.GivenName,
            Surname = user.Contact!.Surname,
            PhoneNumber = user.Contact!.PhoneNumber,
            AccessEvents = accessEvents,
            Orders = userOrders,
            UserId = user.UserId
        };
        
        // Send the model to the view for rendering
        return View(model);
    }

    /// <summary>
    /// Validate form input and update user's details if everything is valid.
    /// </summary>
    /// <param name="model">Model from Settings View</param>
    /// <returns>Settings View with success or error messages attached</returns>
    [HttpPost]
    [AuthenticationFilter]
    public async Task<IActionResult> Settings(UserSettingsViewModel model)
    {
        // Check error state of the model, and redirect to index if the model is malformed
        if (!ModelState.IsValid)
        {
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                logger.LogError(error.ErrorMessage);
            }
            logger.LogError("Register was passed with an invalid model");
            return View(model);
        }
        
        var user = await userRepository.LoadFullUser(model.UserId);

        if (user == null)
        {
            logger.LogError("Authenticated a user settings update without a valid user ID");
            return RedirectToAction("Error", "Home");
        }
        
        if (model.NewPassword != null) // Change their password
        {
            if (model.OldPassword == null) // They need to entire their old password to change their password
            {
                ModelState.AddModelError("needOldPassword", "Please enter your current password to change it.");
                return View(model);
            }
            // Check if their old password matches
            if (HashUtils.VerifyPassword(user.PasswordHash!, model.OldPassword, user.PasswordSalt!))
            {
                // If it does, hash the new password and update the user entity with it
                var passwordHash = HashUtils.HashPassword(model.NewPassword, out var salt);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = salt;
            }
            else
            {
                ModelState.AddModelError("incorrectOldPassword", "Incorrect current password. Please try again.");
                return View(model);
            }
        }

        if (model.Email != null) // Change their email
        {
            // Check if the email is already in use
            var emailMatch = await userRepository.GetByEmail(model.Email);
            if (emailMatch != null && emailMatch.UserId != user.UserId) // This email belongs to a different user
            {
                // If it is, send them back with an error
                ModelState.AddModelError("emailInUse", "This email is unavailable, please choose a different one.");
                return View(model);
            }

            // Update the user entity with the new email
            user.Contact!.Email = model.Email;
        }

        // Check if their new phone number is digits only and exactly 10 digits long
        if (!string.IsNullOrEmpty(model.PhoneNumber))
        {
            if (!Regex.IsMatch(model.PhoneNumber, @"^\d{10}$"))
            {
                ModelState.AddModelError("invalidPhoneNumber", "Your phone number must contain only numeric characters, and must be 10 digits long.");
                return View(model);
            }
        }
        
        // Update their phone number (allow null values, if they want to remove their phone number from their account).
        user.Contact!.PhoneNumber = model.PhoneNumber;

        // Update their given name if it is not null. Do not allow empty names.
        if (model.GivenName != null)
        {
            user.Contact!.GivenName = model.GivenName;
        }
        
        // Update their surname.
        user.Contact!.Surname = model.Surname;

        // Track and update the user entity in the current repository context
        userRepository.Update(user);
        await userRepository.SaveChangesAsync();

        ViewData["SuccessMessage"] = "Details updated successfully!";
        return View(model);
    }
}