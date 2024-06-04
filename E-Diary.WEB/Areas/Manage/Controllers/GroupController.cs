using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using E_Diary.WEB.Data;
using E_Diary.WEB.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using E_Diary.WEB.Helpers;
using Microsoft.Data.SqlClient;
using E_Diary.WEB.Areas.Manage.Models;

namespace E_Diary.WEB.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class GroupController : Controller
    {
        private readonly ASPIdentityDBContext _context;
        private readonly UserManager<User> _userManager;
        public GroupController(ASPIdentityDBContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Group
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Index(int? pageNumber)
        {
            var groups = await _context.Groups.ToListAsync();
            int pageSize = 10;
            return View(await PaginatedList<Group>.CreateAsync(groups, pageNumber ?? 1, pageSize));
        }

        // GET: Group/Details/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @group = await _context.Groups
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@group == null)
            {
                return NotFound();
            }
            List<Data.Entities.Student> studentsInGroup = await _context.Students.Where(
    s => s.Group.Id == group.Id).ToListAsync();
            GroupViewModel model = new GroupViewModel()
            {
                Id = group.Id,
                Year = group.Year,
                Literal = group.Literal,
                Students = studentsInGroup,
                ClassroomTeacherId = group.ClassroomTeacher.Id
            };
            return View(model);
        }

        // GET: Group/Create
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            return View(new GroupViewModel() { Year = 1, Literal = 'А'});
        }

        // POST: Group/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create(GroupViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Data.Entities.Teacher? teacher = await _context.Teachers.FindAsync(model.ClassroomTeacherId);
                    if(teacher == null)
                    {
                        ModelState.AddModelError(string.Empty, "Такого учителя не существует");
                        throw new Exception();
                    }
                    Group group = new()
                    {
                        Year = model.Year,
                        Literal = model.Literal,
                        ClassroomTeacher = teacher
                    };
                    _context.Add(group);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "Невозможно добавить такой класс! Проверьте данные";
                    return View(model);
                }
            }
            return View(model);
        }

        // GET: Group/Edit/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @group = await _context.Groups.FindAsync(id);
            if (@group == null)
            {
                return NotFound();
            }
            GroupViewModel model = new GroupViewModel()
            {
                Id = group.Id,
                Year = group.Year,
                Literal = group.Literal,
                ClassroomTeacherId = group.ClassroomTeacher.Id
            };
            return View(model);
        }

        // POST: Group/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(GroupViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Group? group = await _context.Groups.FindAsync(model.Id);
                    Data.Entities.Teacher? teacher = await _context.Teachers.FindAsync(model.ClassroomTeacherId);
                    if( teacher == null) return NotFound();
                    if (group == null) { return NotFound(); }
                    group.Literal = model.Literal;
                    group.Year = model.Year;
                    group.ClassroomTeacher = teacher;
                    _context.Entry(group).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GroupExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "Невозможно изменить класс. Проверьте данные";
                    return View(model);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Group/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @group = await _context.Groups
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@group == null)
            {
                return NotFound();
            }
            List<Data.Entities.Student> studentsInGroup = await _context.Students.Where(
                s => s.Group.Id == group.Id).ToListAsync();
            GroupViewModel model = new GroupViewModel()
            {
                Id = group.Id,
                Year = group.Year,
                Literal = group.Literal,
                Students = studentsInGroup,
                ClassroomTeacherId = group.ClassroomTeacher.Id
            };
            return View(model);
        }

        // POST: Group/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @group = await _context.Groups.FindAsync(id);
            if (@group != null)
            {
                List<User> studentsInGroup = await _context.Students.Where(
    s => s.Group.Id == group.Id).Select(s => s.User).ToListAsync();
                _context.Groups.Remove(@group);
                await _context.SaveChangesAsync();
                foreach (var student in studentsInGroup)
                {
                    await _userManager.DeleteAsync(student);
                }
            }
            return RedirectToAction(nameof(Index));
        }

        private bool GroupExists(int id)
        {
            return _context.Groups.Any(e => e.Id == id);
        }
    }
}
