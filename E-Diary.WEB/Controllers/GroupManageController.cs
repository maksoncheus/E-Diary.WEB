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
using E_Diary.WEB.Models;
using Microsoft.AspNetCore.Identity;
using E_Diary.WEB.Helpers;
using Microsoft.Data.SqlClient;

namespace E_Diary.WEB.Controllers
{
    public class GroupManageController : Controller
    {
        private readonly ASPIdentityDBContext _context;
        private readonly UserManager<User> _userManager;
        public GroupManageController(ASPIdentityDBContext context, UserManager<User> userManager)
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
            List<Student> studentsInGroup = await _context.Students.Where(
    s => s.Group.Id == group.Id).ToListAsync();
            GroupViewModel model = new GroupViewModel()
            {
                Id = group.Id,
                Year = group.Year,
                Literal = group.Literal,
                Students = studentsInGroup
            };
            return View(model);
        }

        // GET: Group/Create
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Group/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([Bind("Id,Year,Literal")] Group @group)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(@group);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "Невозможно добавить такой класс! Проверьте данные";
                    return View(@group);
                }
            }
            return View(@group);
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
            return View(@group);
        }

        // POST: Group/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Year,Literal")] Group @group)
        {
            if (id != @group.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@group);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GroupExists(@group.Id))
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
                    return View(@group);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(@group);
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
            List<Student> studentsInGroup = await _context.Students.Where(
                s => s.Group.Id == group.Id).ToListAsync();
            GroupViewModel model = new GroupViewModel()
            {
                Id = group.Id,
                Year = group.Year,
                Literal = group.Literal,
                Students = studentsInGroup
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
