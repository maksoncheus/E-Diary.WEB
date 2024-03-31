using E_Diary.WEB.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Diary.WEB.Controllers
{
    public class TeacherController : Controller
    {
        private readonly ASPIdentityDBContext _context;
        public TeacherController(ASPIdentityDBContext context)
        {
            _context = context;
        }
        [Authorize(Roles ="teacher")]
        public IActionResult Index(string? date)
        {
            DateOnly dateOnly = date != null ? DateOnly.Parse(date) : DateOnly.FromDateTime(DateTime.Now);
            return View(dateOnly);
        }
    }
}
