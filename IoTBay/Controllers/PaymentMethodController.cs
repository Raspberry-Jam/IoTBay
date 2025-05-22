using IoTBay.Models;
using IoTBay.Models.Entities;
using IoTBay.Models.DTOs;
using IoTBay.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using IoTBay.Utils;

namespace IoTBay.Controllers
{
    public class BankInfo
    {
        public string Name { get; set; }
        public string IconPath { get; set; }
        public string Code { get; set; }
        public List<string> Bins { get; set; } = new();
    }

    [AuthenticationFilter] // 仅登录用户可访问
    public class PaymentMethodController : Controller
    {
        private readonly AppDbContext _context;

        public PaymentMethodController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string mode = "view", string? returnUrl = null)
        {
            var sessionDto = SessionUtils.GetObjectFromJson<UserSessionDto>(HttpContext.Session, "currentUser");
            if (sessionDto == null) return RedirectToAction("Index", "Login");

            int userId = sessionDto.UserId;

            var methods = _context.PaymentMethods
                .Where(pm => pm.UserId == userId)
                .ToList();

            var bankBins = GetBankList()
                .SelectMany(bank => bank.Bins.Select(bin => new { bin, bank }))
                .ToDictionary(x => x.bin, x => x.bank);

            ViewBag.BankBins = bankBins;
            ViewBag.Mode = mode;
            ViewBag.ReturnUrl = returnUrl;

            if (TempData["SuccessMessage"] != null)
                ViewBag.SuccessMessage = TempData["SuccessMessage"];

            return View(methods);
        }

        public IActionResult SelectBank(string? query, string mode = "view", string? returnUrl = null)
        {
            var banks = GetBankList();

            if (!string.IsNullOrWhiteSpace(query))
            {
                query = query.Trim().ToLower();
                banks = banks.Where(b =>
                    b.Name.ToLower().Contains(query) ||
                    b.Code.ToLower().Contains(query) ||
                    b.Bins.Any(bin => query.StartsWith(bin))
                ).ToList();
            }

            ViewBag.Mode = mode;
            ViewBag.ReturnUrl = returnUrl;
            return View(banks);
        }

        public IActionResult AddCardDetails(string bank, string? card, string mode = "view", string? returnUrl = null)
        {
            var bankInfo = GetBankList().FirstOrDefault(b => b.Code == bank);
            if (bankInfo == null) return NotFound();

            ViewBag.BankName = bankInfo.Name;
            ViewBag.BankIcon = bankInfo.IconPath;
            ViewBag.CardNumber = card;
            ViewBag.Bins = string.Join(",", bankInfo.Bins);
            ViewBag.Mode = mode;
            ViewBag.ReturnUrl = returnUrl;

            return View();
        }

        [HttpPost]
        public IActionResult AddCardDetails(IFormCollection form)
        {
            var sessionDto = SessionUtils.GetObjectFromJson<UserSessionDto>(HttpContext.Session, "currentUser");
            if (sessionDto == null) return RedirectToAction("Index", "Login");
            int userId = sessionDto.UserId;

            var rawCard = form["CardNumber"];
            var cardNumber = new string(rawCard.ToString().Where(char.IsDigit).ToArray());

            var cvv = form["Cvv"];
            var expiryString = form["Expiry"];

            var mode = form["Mode"].ToString();
            var returnUrl = form["ReturnUrl"].ToString();

            DateOnly expiry;
            if (!DateTime.TryParse(expiryString + "-01", out var parsedDate))
            {
                ModelState.AddModelError("Expiry", "Invalid expiry date.");
                expiry = DateOnly.FromDateTime(DateTime.Now);
            }
            else
            {
                expiry = DateOnly.FromDateTime(parsedDate);
            }

            var method = new PaymentMethod
            {
                CardNumber = cardNumber,
                Cvv = cvv,
                Expiry = expiry,
                UserId = userId
            };

            if (ModelState.IsValid)
            {
                _context.PaymentMethods.Add(method);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Card added successfully!";

                if (mode == "select")
                {
                    return RedirectToAction("Index", new { mode = "select", returnUrl });
                }

                return RedirectToAction("Index");
            }

            ViewBag.CardNumber = method.CardNumber;
            var bankInfo = GetBankList().FirstOrDefault(b => method.CardNumber?.StartsWith(b.Bins.FirstOrDefault()) == true);
            if (bankInfo != null)
            {
                ViewBag.BankName = bankInfo.Name;
                ViewBag.BankIcon = bankInfo.IconPath;
                ViewBag.Bins = string.Join(",", bankInfo.Bins);
            }
            ViewBag.Mode = mode;
            ViewBag.ReturnUrl = returnUrl;

            return View(method);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var sessionDto = SessionUtils.GetObjectFromJson<UserSessionDto>(HttpContext.Session, "currentUser");
            if (sessionDto == null) return RedirectToAction("Index", "Login");

            var card = _context.PaymentMethods.FirstOrDefault(c => c.PaymentMethodId == id && c.UserId == sessionDto.UserId);
            if (card != null)
            {
                _context.PaymentMethods.Remove(card);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Card deleted successfully.";
            }
            return RedirectToAction("Index");
        }

        private List<BankInfo> GetBankList()
        {
            return new List<BankInfo>
            {
                new BankInfo { Name = "Commonwealth Bank", IconPath = "/images/banks/cba.png", Code = "CBA", Bins = new List<string> { "5123", "5124" } },
                new BankInfo { Name = "Westpac", IconPath = "/images/banks/westpac.png", Code = "WBC", Bins = new List<string> { "4001", "4002" } },
                new BankInfo { Name = "NAB", IconPath = "/images/banks/nab.png", Code = "NAB", Bins = new List<string> { "5310", "5311" } },
                new BankInfo { Name = "ANZ", IconPath = "/images/banks/anz.png", Code = "ANZ", Bins = new List<string> { "4200", "4211" } }
            };
        }
    }
}


