using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using E_Diary.WEB.Data;
using E_Diary.WEB.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using E_Diary.WEB.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using E_Diary.WEB.Data.Enums;
using E_Diary.WEB.Areas.Manage.Models;

namespace E_Diary.WEB.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class StudentController : Controller
    {
        private readonly ASPIdentityDBContext _context;
        private readonly UserManager<User> _userManager;

        public StudentController(ASPIdentityDBContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Student
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

            var students = await _context.Students.ToListAsync();
            if (!string.IsNullOrEmpty(searchString))
            {
                students = students.Where(s => s.User.Surname.Contains(searchString)).ToList();
            }
            switch (sortOrder)
            {
                case "name_desc":
                    students = students.OrderByDescending(s => s.User.Surname).ToList();
                    break;
                case "Date":
                    students = students.OrderBy(s => s.Group.Year.ToString() + s.Group.Literal).ToList();
                    break;
                case "date_desc":
                    students = students.OrderByDescending(s => s.Group.Year + s.Group.Literal).ToList();
                    break;
                default:
                    students = students.OrderBy(s => s.User.Surname).ToList();
                    break;
            }

            int pageSize = 10;
            return View(await PaginatedList<Data.Entities.Student>.CreateAsync(students, pageNumber ?? 1, pageSize));
        }

        // GET: Student/Details/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Student/Create
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Student/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create(StudentViewModel student)
        {
            if (ModelState.IsValid)
            {
                Group? group = await _context.Groups.FindAsync(student.GroupId);
                if (group != null)
                {
                    User user = new User()
                    {
                        Name = student.Name,
                        Surname = student.Surname,
                        Patronymic = student.Patronymic,
                        Email = student.EmailAddress,
                        Gender = (Gender)student.Gender,
                        UserName = student.EmailAddress
                    };
                    var result = await _userManager.CreateAsync(user, student.Password);
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "schoolboy");
                        Data.Entities.Student _student = new Data.Entities.Student()
                        {
                            Group = group,
                            User = user
                        };
                        await _context.Students.AddAsync(_student);
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
            }
            return View(student);
        }

        // GET: Student/Edit/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            StudentEditViewModel studentVM = new()
            {
                Id = student.Id,
                Name = student.User.Name,
                Surname = student.User.Surname,
                Patronymic = student.User.Patronymic,
                Gender = (int)student.User.Gender,
                EmailAddress = student.User.Email,
                GroupId = student.Group.Id,
                UserId = student.User.Id
            };
            return View(studentVM);
        }

        // POST: Student/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(StudentEditViewModel student)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Data.Entities.Student? _student = await _context.Students.FindAsync(student.Id);
                    User? _user = await _userManager.FindByIdAsync(student.UserId);
                    Group? _group = await _context.Groups.FindAsync(student.GroupId);
                    if (_user != null && _group != null && _student != null)
                    {
                        _user.Name = student.Name;
                        _user.Surname = student.Surname;
                        _user.Patronymic = student.Patronymic;
                        _user.Gender = (Gender)student.Gender;
                        _context.Entry(_user).State = EntityState.Modified;
                        _student.Group = _group;
                        _context.Entry(_student).State = EntityState.Modified;

                    }
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(student);
        }
        // GET: Student/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Student/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                User user = student.User;
                _context.Students.Remove(student);
                await _userManager.DeleteAsync(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.Id == id);
        }
    }
}
