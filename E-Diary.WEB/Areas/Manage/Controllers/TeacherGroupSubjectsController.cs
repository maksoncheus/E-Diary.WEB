using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using E_Diary.WEB.Data;
using E_Diary.WEB.Data.Entities;
using E_Diary.WEB.Helpers;
using Microsoft.AspNetCore.Authorization;
using E_Diary.WEB.Areas.Manage.Models;

namespace E_Diary.WEB.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class TeacherGroupSubjectsController : Controller
    {
        private readonly ASPIdentityDBContext _context;

        public TeacherGroupSubjectsController(ASPIdentityDBContext context)
        {
            _context = context;
        }

        // GET: TeacherGroupSubjects
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Index(int? pageNumber)
        {
            var subjects = await _context.TeacherGroupSubjects.ToListAsync();
            int pageSize = 10;
            return View(await PaginatedList<TeacherGroupSubject>.CreateAsync(subjects, pageNumber ?? 1, pageSize));
        }

        // GET: TeacherGroupSubjects/Create
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: TeacherGroupSubjects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create(TeacherGroupSubjectViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var entry = await _context.TeacherGroupSubjects.FirstOrDefaultAsync(
                    e =>
                        e.Teacher.Id == vm.TeacherId
                        &&
                        e.Group.Id == vm.GroupId
                        &&
                        e.Subject.Id == vm.SubjectId
                        &&
                        e.StudyYear.Id == vm.StudyYearId
                    );
                if (entry == null)
                {
                    Group? group = await _context.Groups.FirstOrDefaultAsync(g => g.Id == vm.GroupId);
                    Data.Entities.Teacher? teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.Id == vm.TeacherId);
                    Subject? subject = await _context.Subjects.FirstOrDefaultAsync(s => s.Id == vm.SubjectId);
                    StudyYear? year = await _context.StudyYears.FirstOrDefaultAsync(y => y.Id == vm.StudyYearId);
                    if (group != null && teacher != null && subject != null && year != null)
                    {
                        TeacherGroupSubject entity = new()
                        {
                            Teacher = teacher,
                            Subject = subject,
                            Group = group,
                            StudyYear = year
                        };
                        await _context.AddAsync(entity);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Вы не можете указать такие данные");

                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Такая комбинация уже существует");
                }
            }
            return View(vm);
        }

        // GET: TeacherGroupSubjects/Edit/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacherGroupSubject = await _context.TeacherGroupSubjects.FindAsync(id);
            if (teacherGroupSubject == null)
            {
                return NotFound();
            }
            TeacherGroupSubjectViewModel vm = new()
            {
                Id = (int)id,
                TeacherId = teacherGroupSubject.Teacher.Id,
                GroupId = teacherGroupSubject.Group.Id,
                SubjectId = teacherGroupSubject.Subject.Id,
                StudyYearId = teacherGroupSubject.StudyYear.Id
            };
            return View(vm);
        }

        // POST: TeacherGroupSubjects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TeacherGroupSubjectViewModel vm)
        {
            if (id != vm.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var editedEntity = await _context.TeacherGroupSubjects.FindAsync(vm.Id);
                    if(editedEntity == null) return NotFound();
                    var possibleCollision = await _context.TeacherGroupSubjects.FirstOrDefaultAsync(
                        e =>
                            e.Teacher.Id == vm.TeacherId
                            &&
                            e.Group.Id == vm.GroupId
                            &&
                            e.Subject.Id == vm.SubjectId
                            &&
                            e.StudyYear.Id == vm.StudyYearId
                        );
                    if (possibleCollision == null || possibleCollision.Id == editedEntity.Id)
                    {
                        Group? group = await _context.Groups.FirstOrDefaultAsync(g => g.Id == vm.GroupId);
                        Data.Entities.Teacher? teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.Id == vm.TeacherId);
                        Subject? subject = await _context.Subjects.FirstOrDefaultAsync(s => s.Id == vm.SubjectId);
                        StudyYear? year = await _context.StudyYears.FirstOrDefaultAsync(y => y.Id == vm.StudyYearId);
                        if (group != null && teacher != null && subject != null && year != null)
                        {
                            editedEntity.Subject = subject;
                            editedEntity.Teacher = teacher;
                            editedEntity.Group = group;
                            editedEntity.StudyYear = year;
                            _context.Entry(editedEntity).State = EntityState.Modified;
                            await _context.SaveChangesAsync();
                            return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Вы не можете указать такие данные");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Такая комбинация уже существует");
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeacherGroupSubjectExists(vm.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(vm);
        }

        // GET: TeacherGroupSubjects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacherGroupSubject = await _context.TeacherGroupSubjects
                .FirstOrDefaultAsync(m => m.Id == id);
            if (teacherGroupSubject == null)
            {
                return NotFound();
            }

            return View(teacherGroupSubject);
        }

        // POST: TeacherGroupSubjects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var teacherGroupSubject = await _context.TeacherGroupSubjects.FindAsync(id);
            if (teacherGroupSubject != null)
            {
                _context.TeacherGroupSubjects.Remove(teacherGroupSubject);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TeacherGroupSubjectExists(int id)
        {
            return _context.TeacherGroupSubjects.Any(e => e.Id == id);
        }
    }
}
