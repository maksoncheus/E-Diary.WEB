using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using E_Diary.WEB.Data;
using E_Diary.WEB.Data.Entities;
using E_Diary.WEB.Areas.Manage.Models;
using E_Diary.WEB.Data.Enums;
using E_Diary.WEB.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Text;

namespace E_Diary.WEB.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class ParentController : Controller
    {
        private readonly ASPIdentityDBContext _context;
        private readonly UserManager<User> _userManager;

        public ParentController(
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

            var parents = await _context.Parents.ToListAsync();
            if (!string.IsNullOrEmpty(searchString))
            {
                parents = parents.Where(s => s.User.Surname.Contains(searchString)).ToList();
            }
            switch (sortOrder)
            {
                case "name_desc":
                    parents = parents.OrderByDescending(s => s.User.Surname).ToList();
                    break;
                default:
                    parents = parents.OrderBy(s => s.User.Surname).ToList();
                    break;
            }

            int pageSize = 10;
            return View(await PaginatedList<Data.Entities.Parent>.CreateAsync(parents, pageNumber ?? 1, pageSize));
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var parent = await _context.Parents
                .FirstOrDefaultAsync(m => m.Id == id);
            if (parent == null)
            {
                return NotFound();
            }

            return View(parent);
        }

        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create(ParentViewModel parent)
        {
            if (ModelState.IsValid)
            {
                User user = new User()
                {
                    Name = parent.Name,
                    Surname = parent.Surname,
                    Patronymic = parent.Patronymic,
                    Email = parent.EmailAddress,
                    Gender = (Gender)parent.Gender,
                    UserName = parent.EmailAddress
                };
                var result = await _userManager.CreateAsync(user, parent.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "parent");
                    List<Data.Entities.Student> children = new();
                    Data.Entities.Parent _parent = new Data.Entities.Parent()
                    {
                        User = user,
                        Children = children
                    };
                    await _context.Parents.AddAsync(_parent);
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
            return View(parent);
        }

        // GET: Teachers/Edit/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            var parent = await _context.Parents.FindAsync(id);
            if (parent == null)
            {
                return NotFound();
            }
            ParentEditViewModel parentVM = new()
            {
                Id = parent.Id,
                Name = parent.User.Name,
                Surname = parent.User.Surname,
                Patronymic = parent.User.Patronymic,
                Gender = (int)parent.User.Gender,
                EmailAddress = parent.User.Email,
                UserId = parent.User.Id
            };
            return View(parentVM);
        }

        // POST: Teachers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(ParentEditViewModel parent)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Data.Entities.Parent? _parent = await _context.Parents.FindAsync(parent.Id);
                    User? _user = await _userManager.FindByIdAsync(parent.UserId);
                    if (_user != null && _parent != null)
                    {
                        _user.Name = parent.Name;
                        _user.Surname = parent.Surname;
                        _user.Patronymic = parent.Patronymic;
                        _user.Gender = (Gender)parent.Gender;
                        _context.Entry(_user).State = EntityState.Modified;
                        _context.Entry(_parent).State = EntityState.Modified;

                    }
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeacherExists(parent.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(parent);
        }

        // GET: Teachers/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var parent = await _context.Parents
                .FirstOrDefaultAsync(m => m.Id == id);
            if (parent == null)
            {
                return NotFound();
            }

            return View(parent);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var parent = await _context.Parents.FindAsync(id);
            if (parent != null)
            {
                User user = parent.User;
                _context.Parents.Remove(parent);
                await _userManager.DeleteAsync(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TeacherExists(int id)
        {
            return _context.Parents.Any(e => e.Id == id);
        }
        [HttpGet]
        public async Task<string> GetChildrenForm(int parentId)
        {
            try
            {
                Data.Entities.Parent? parent = await _context.Parents.FindAsync(parentId);
                if (parent == null) return string.Empty;
                StringBuilder sb = new();
                sb.AppendLine($"<input type=\"hidden\" name = \"parentId\" value=\"{parentId}\" />");
                sb.AppendLine("<div class=\"form-group\">");
                sb.AppendLine("<label class=\"control-label\">Ученик</label>");
                sb.AppendLine("<select name=\"studentId\" class=\"form-control\" data-live-search=\"true\" required>");
                foreach (var child in _context.Students.AsEnumerable().Except(parent.Children))
                {
                    sb.AppendLine($"<option data-tokens=\"{child.Group.Year} {child.Group.Literal} {child.User.Surname} {child.User.Name} {child.User.Patronymic}\" value=\"{child.Id}\">{child.Group.Year} {child.Group.Literal} {child.User.Surname} {child.User.Name} {child.User.Patronymic}</option>");
                }
                sb.AppendLine("</select>");
                sb.AppendLine("</div>");
                return sb.ToString();
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddChild(int parentId, int studentId)
        {
            Data.Entities.Parent? parent = await _context.Parents.FindAsync(parentId);
            Data.Entities.Student? student = await _context.Students.FindAsync(studentId);
            if (parent != null && student != null)
            {
                parent.Children.Add(student);
                _context.Entry(parent).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok();
            }
            else
                return BadRequest("Не удалось добавить ребенка");
        }
        [HttpPost]
        public async Task<IActionResult> RemoveChild(int parentId, int studentId)
        {
            Data.Entities.Parent? parent = await _context.Parents.FindAsync(parentId);
            Data.Entities.Student? student = await _context.Students.FindAsync(studentId);
            if (parent != null && student != null)
            {
                parent.Children.Remove(student);
                _context.Entry(parent).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok();
            }
            else
                return BadRequest("Не удалось удалить ребенка");
        }
    }
}
