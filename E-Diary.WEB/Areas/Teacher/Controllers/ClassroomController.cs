using E_Diary.WEB.Areas.Manage.Models;
using E_Diary.WEB.Data;
using E_Diary.WEB.Data.Entities;
using E_Diary.WEB.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Diary.WEB.Areas.Teacher.Controllers
{
    [Area("Teacher")]
    public class ClassroomController : Controller
    {
        private readonly ASPIdentityDBContext _context;
        private readonly UserManager<User> _userManager;
        public ClassroomController(ASPIdentityDBContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [HttpGet]
        [Authorize(Roles = "teacher")]
        public async Task<IActionResult> Group(int? id)
        {
            Group? group = null;
            Data.Entities.Teacher? teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.User.Id == _userManager.GetUserAsync(HttpContext.User).Result.Id);
            if(id == null)
            {
                group = await _context.Groups.FirstOrDefaultAsync(g => g.ClassroomTeacher.Id == teacher.Id);
            }
            else group = await _context.Groups.FindAsync(id);
            if (group == null || group.ClassroomTeacher.Id != teacher.Id) return NotFound();
            List<Data.Entities.Student> students = await _context.Students.Where(s => s.Group.Id == group.Id).ToListAsync();
            GroupViewModel model = new()
            {
                Id = group.Id,
                Year = group.Year,
                Literal = group.Literal,
                Students = students
            };
            return View(model);
        }
    }
}
