using E_Diary.WEB.Areas.Manage.Models;
using E_Diary.WEB.Data.Entities;
using E_Diary.WEB.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Diary.WEB.Areas.Parent.Controllers
{
    [Area("Parent")]
    public class ChildrenController : Controller
    {
        private readonly ASPIdentityDBContext _context;
        private readonly UserManager<User> _userManager;
        public ChildrenController(ASPIdentityDBContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [HttpGet]
        [Authorize(Roles = "parent")]
        public async Task<IActionResult> Index(int? id)
        {
            Data.Entities.Parent? parent = await _context.Parents.FirstOrDefaultAsync(t => t.User.Id == _userManager.GetUserAsync(HttpContext.User).Result.Id);
            if (parent == null) return NotFound();
            List<Data.Entities.Student> students = await _context.Students.Where(s => s.Parents.Contains(parent)).ToListAsync();
            return View(students);
        }
    }
}
