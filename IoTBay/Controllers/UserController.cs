using IoTBay.Models.DTOs;
using IoTBay.Models.Entities;
using IoTBay.Models.Views;
using IoTBay.Repositories;
using IoTBay.Utils;
using Microsoft.AspNetCore.Mvc;

namespace IoTBay.Controllers;

public class UserController(ILogger<UserController> logger, UserRepository userRepository)
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
        SessionUtils.SetObjectAsJson(HttpContext.Session, "currentUser", userSessionDto);
        
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

        // Send the user to the welcome page
        return View("Welcome", user);
    }
    
    // Allow both HttpGet and HttpPost
    /// <summary>
    /// Clear the session data of this user, logging them out. Only allow this action if the user is already logged in.
    /// </summary>
    /// <returns>Home Index View</returns>
    [AuthenticationFilter]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }
}