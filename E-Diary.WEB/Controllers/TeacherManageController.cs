using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using E_Diary.WEB.Data;
using E_Diary.WEB.Data.Entities;
using Microsoft.AspNetCore.Identity;
using E_Diary.WEB.Models;
using E_Diary.WEB.Data.Enums;
using Microsoft.AspNetCore.Authorization;
using E_Diary.WEB.Helpers;

namespace E_Diary.WEB.Controllers
{
    public class TeacherManageController : Controller
    {
        private readonly ASPIdentityDBContext _context;
        private readonly UserManager<User> _userManager;

        public TeacherManageController(
            ASPIdentityDBContext context,
            UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Teachers
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Index(
        string sortOrder,
        string currentFilter,
        string searchString,
        int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var teachers = await _context.Teachers.ToListAsync();
            if (!String.IsNullOrEmpty(searchString))
            {
                teachers = teachers.Where(s => s.User.Surname.Contains(searchString)).ToList();
            }
            switch (sortOrder)
            {
                case "name_desc":
                    teachers = teachers.OrderByDescending(s => s.User.Surname).ToList();
                    break;
                default:
                    teachers = teachers.OrderBy(s => s.User.Surname).ToList();
                    break;
            }

            int pageSize = 10;
            return View(await PaginatedList<Teacher>.CreateAsync(teachers, pageNumber ?? 1, pageSize));
        }

        // GET: Teachers/Details/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teachers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

        // GET: Teachers/Create
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Teachers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create(TeacherViewModel teacher)
        {
            if (ModelState.IsValid)
            {
                User user = new User()
                {
                    Name = teacher.Name,
                    Surname = teacher.Surname,
                    Patronymic = teacher.Patronymic,
                    Email = teacher.EmailAddress,
                    Gender = (Data.Enums.Gender)teacher.Gender,
                    UserName = teacher.EmailAddress
                };
                var result = await _userManager.CreateAsync(user, teacher.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "teacher");
                    Teacher _teacher = new Teacher()
                    {
                        User = user
                    };
                    await _context.Teachers.AddAsync(_teacher);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(teacher);
        }

        // GET: Teachers/Edit/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null)
            {
                return NotFound();
            }
            TeacherEditViewModel teacherVM = new()
            {
                Id = teacher.Id,
                Name = teacher.User.Name,
                Surname = teacher.User.Surname,
                Patronymic = teacher.User.Patronymic,
                Gender = (int)teacher.User.Gender,
                EmailAddress = teacher.User.Email,
                UserId = teacher.User.Id
            };
            return View(teacherVM);
        }

        // POST: Teachers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(TeacherEditViewModel teacher)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Teacher? _teacher = await _context.Teachers.FindAsync(teacher.Id);
                    User? _user = await _userManager.FindByIdAsync(teacher.UserId);
                    if (_user != null && _teacher != null)
                    {
                        _user.Name = teacher.Name;
                        _user.Surname = teacher.Surname;
                        _user.Patronymic = teacher.Patronymic;
                        _user.Gender = (Gender)teacher.Gender;
                        _context.Entry(_user).State = EntityState.Modified;
                        _context.Entry(_teacher).State = EntityState.Modified;

                    }
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeacherExists(teacher.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(teacher);
        }

        // GET: Teachers/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teachers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

        // POST: Teachers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher != null)
            {
                User user = teacher.User;
                _context.Teachers.Remove(teacher);
                await _userManager.DeleteAsync(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TeacherExists(int id)
        {
            return _context.Teachers.Any(e => e.Id == id);
        }
    }
}
