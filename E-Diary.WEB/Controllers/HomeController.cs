using E_Diary.WEB.Data;
using E_Diary.WEB.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using E_Diary.WEB.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace E_Diary.WEB.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly ASPIdentityDBContext _context;
        public HomeController(ILogger<HomeController> logger, ASPIdentityDBContext context, UserManager<User> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            string? username = User?.Identity?.Name;
            if (username != null)
            {
                User? user = _context.Users.FirstOrDefault(x => x.UserName == username);
                if (await _userManager.IsInRoleAsync(user, "admin"))
                    return RedirectToAction("Index", "Admin");
                if (await _userManager.IsInRoleAsync(user, "teacher"))
                    return RedirectToAction("Index", "Teacher");
                if (await _userManager.IsInRoleAsync(user, "schoolboy"))
                    return RedirectToAction("Profile", "Account");
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
