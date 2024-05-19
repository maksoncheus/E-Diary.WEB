using E_Diary.WEB.Areas.Manage.Models;
using E_Diary.WEB.Data;
using E_Diary.WEB.Data.Entities;
using E_Diary.WEB.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace E_Diary.WEB.Areas.Manage.Controllers
{
    /// <summary>
    /// This schedule is based on the choice of week and class
    /// </summary>
    [Area("Manage")]
    public class ScheduleController : Controller
    {
        private readonly ASPIdentityDBContext _context;
        public ScheduleController(ASPIdentityDBContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string? date, int? groupId)
        {
            if (groupId == null)
            {
                Group? firstGroup = await _context.Groups.FirstOrDefaultAsync();
                if (firstGroup == null)
                    return BadRequest("Нет ни одного класса");
                groupId = firstGroup.Id;
            }
            DateOnly dateOnly;
            if (date == null) dateOnly = DateOnly.FromDateTime(DateTime.Now.Date);
            else dateOnly = DateOnly.Parse(date);
            DateOnly startOfWeek = dateOnly.StartOfWeek(DayOfWeek.Monday);
            DateOnly endOfWeek = dateOnly.EndOfWeek(DayOfWeek.Monday);
            List<Lesson> lessons = await _context.Lessons.Where(
                l => l.Date >= startOfWeek && l.Date <= endOfWeek
                &&
                    l.LessonInfo.Group.Id == groupId
                ).ToListAsync();
            ScheduleViewModel vm = new()
            {
                StartOfWeek = startOfWeek,
                EndOfWeek = endOfWeek,
                GroupId = (int)groupId,
                Lessons = lessons
            };
            return View(vm);
        }
        public async Task<IActionResult> AddLesson(int lessonNumber, int teacherGroupSubjectId, DateOnly date)
        {
            TeacherGroupSubject? tgs = await _context.TeacherGroupSubjects.FindAsync(teacherGroupSubjectId);
            if (tgs != null)
            {
                Lesson lesson = new()
                {
                    Date = date,
                    LessonInfo = tgs,
                    LessonOnDayNumber = lessonNumber
                };
                await _context.Lessons.AddAsync(lesson);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { groupId = tgs.Group.Id, date = date.ToString() });
            }
            return NotFound();
        }
        public async Task<IActionResult> DeleteLesson(long id, string date, int groupId)
        {
            Lesson? lesson = await _context.Lessons.FindAsync(id);
            if (lesson == null)
                return NotFound();
            _context.Lessons.Remove(lesson);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { groupId, date = date.ToString() });

        }
    }
}
