using IoTBay.Models;
using Microsoft.AspNetCore.Mvc;

namespace IoTBay.Controllers;

public class RegisterController : Controller
{
    private readonly ILogger<RegisterController> _logger;
    private readonly AppDbContext _db;

    public RegisterController(ILogger<RegisterController> logger, AppDbContext db)
    {
        _logger = logger;
        _db = db;
    }
    
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public void Register()
    {
        
    }
}