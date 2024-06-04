using E_Diary.WEB.Areas.Manage.Models;
using E_Diary.WEB.Areas.Student.Models;
using E_Diary.WEB.Data;
using E_Diary.WEB.Data.Entities;
using E_Diary.WEB.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace E_Diary.WEB.Areas.Student.Controllers
{
    [Area("Student")]
    public class HomeController : Controller
    {
        public const string selectedGradePageConst = "border-1 border-dark bg-white border-start border-end border-top ";

        private ASPIdentityDBContext _context;
        private UserManager<User> _userManager;
        public HomeController(ASPIdentityDBContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [Authorize(Roles = "schoolboy")]
        public async Task<IActionResult> Schedule(string? date)
        {
            User? user = await _userManager.GetUserAsync(HttpContext.User);
            if(user == null) { return NotFound(); }
            Data.Entities.Student? student = await _context.Students.FirstOrDefaultAsync(s => s.User.Id == user.Id);
            if(student == null) return NotFound();
            int groupId = student.Group.Id;
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
        public async Task<IActionResult> Grades(int periodId = -1)
        {
            StudyYear? year = _context.StudyYears.OrderBy(y => y.End).LastOrDefault(y => y.Start <= DateOnly.FromDateTime(DateTime.Today));
            int certificationPeriodId = 0;
            //Если аттестационный период не указан в запросе, открывается последний на текущую дату
            if (periodId == -1)
            {
                certificationPeriodId = _context.CertificationPeriods.OrderBy(p => p.End).LastOrDefault(p => p.Start <= DateOnly.FromDateTime(DateTime.Today))?.Id ?? -1;
            }
            else certificationPeriodId = periodId;
            if (certificationPeriodId == -1)
                return NotFound();
            CertificationPeriod period = await _context.CertificationPeriods.FindAsync(certificationPeriodId);
            if (year == null) return NotFound();
            User? user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null) { return NotFound(); }
            Data.Entities.Student? student = await _context.Students.FirstOrDefaultAsync(s => s.User.Id == user.Id);
            if (student == null) return NotFound();
            int groupId = student.Group.Id;
            List<Grade> grades = await _context.Grades
                .Where(
                    g => g.User.Id == user.Id
                    && g.Lesson.LessonInfo.StudyYear.Id == year.Id
                    && g.Lesson.Date >= period.Start
                    && g.Lesson.Date <= period.End)
                .OrderBy(g => g.Lesson.Date)
                .ThenBy(g => g.Lesson.LessonOnDayNumber)
                .ToListAsync();
            List<Subject> subjects = await _context.TeacherGroupSubjects
                .Where(tgs => tgs.Group.Id == groupId && tgs.StudyYear.Id == year.Id)
                .Select(tgs => tgs.Subject)
                .Distinct()
                .ToListAsync();
            return View(new GradesViewModel() { Grades = grades, Subjects = subjects, StudyYear = year, CertificationPeriod = period});
        }
    }
}
