using E_Diary.WEB.Areas.Teacher.Models;
using E_Diary.WEB.Data;
using E_Diary.WEB.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace E_Diary.WEB.Areas.Teacher.Controllers
{
    [Area("Teacher")]
    public class JournalController : Controller
    {
        private readonly ASPIdentityDBContext _context;
        public JournalController(ASPIdentityDBContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "teacher")]
        public IActionResult Main(string? date)
        {
            DateOnly dateOnly = date != null ? DateOnly.Parse(date) : DateOnly.FromDateTime(DateTime.Now);
            return View(dateOnly);
        }
        [Authorize(Roles = "teacher")]
        public IActionResult Grades(int? groupId, int? subjectId, int periodId = -1)
        {
            Data.Entities.Teacher? teacher = _context.Teachers.FirstOrDefault(
                t => t.User.UserName == HttpContext.User.Identity.Name);
            if (teacher == null)
                return Forbid();

            IEnumerable<TeacherGroupSubject> tgss = _context.TeacherGroupSubjects
                .Where(tgs => tgs.Teacher.Id == teacher.Id);

            if (tgss.Count() == 0)
                return NotFound();

            int group = groupId == null ? tgss.First().Group.Id : groupId.Value;
            tgss = tgss.Where(tgs => tgs.Group.Id == group);
            if (tgss.Count() == 0)
                return NotFound();
            int subject = subjectId == null ? tgss.First().Subject.Id : subjectId.Value;
            TeacherGroupSubject? tgs = tgss.FirstOrDefault(tgs =>
            tgs.Group.Id == group && tgs.Subject.Id == subject);

            if (tgs == null)
                return NotFound();
            int certificationPeriodId = 0;
            if (periodId == -1)
            {
                certificationPeriodId = _context.CertificationPeriods.FirstOrDefault(p => p.Start <= DateOnly.FromDateTime(DateTime.Today) && p.End >= DateOnly.FromDateTime(DateTime.Today))?.Id ?? 0;
            }
            else certificationPeriodId = periodId;
            GradesViewModel model = new()
            {
                TeacherGroupSubjectId = tgs.Id,
                CertificationPeriodId = certificationPeriodId
            };
            return View(model);
        }
        [HttpPost]
        [Authorize(Roles = "teacher")]
        public async Task<string> RemoveGrade(int studentId, long lessonId)
        {
            Lesson? lesson = await _context.Lessons.FindAsync(lessonId);
            if (lesson == null) return "";
            Student? student = await _context.Students.FindAsync(studentId);
            if (student == null) return "";
            Grade? grade = await _context.Grades.FirstOrDefaultAsync(g => g.Lesson.Id == lesson.Id && g.User.Id == student.User.Id);
            if (grade != null)
            {
                _context.Grades.Remove(grade);
                await _context.SaveChangesAsync();
            }
            return "<span class=\"badge badge-grey\"></span>\r\n<p class=\"text-danger m-0 badge-missed\" style=\"position:absolute; top:70%; left:5%; font-size:0.7rem;\">\r\n</p>";

        }
        [HttpPost]
        [Authorize(Roles = "teacher")]
        public async Task<string> SetGrade(int studentId, long lessonId, string grade, bool isMissed)
        {
            bool needToChange = false;
            Lesson? lesson = await _context.Lessons.FindAsync(lessonId);
            Student? student = await _context.Students.FindAsync(studentId);
            if (student == null)
            {
                return "";
            }
            if (lesson == null) return "";
            Grade? newGrade = await _context.Grades.FirstOrDefaultAsync(g => g.Lesson.Id == lesson.Id && g.User.Id == student.User.Id);
            if (newGrade == null)
                newGrade = new();
            else needToChange = true;
            if (grade == null)
            {
                newGrade.Value = null;
            }
            else
                newGrade.Value = grade;
            newGrade.IsMissed = isMissed;
            if (needToChange)
            {
                _context.Entry(newGrade).State = EntityState.Modified;
            }
            else
            {
                newGrade.Lesson = lesson;
                newGrade.User = student.User;
                _context.Grades.Add(newGrade);
            }
            await _context.SaveChangesAsync();
            string mockup = $"<span class=\"badge badge-grey\">{newGrade?.Value ?? ""}</span>\r\n<p class=\"text-danger m-0 badge-missed\" style=\"position:absolute; top:70%; left:5%; font-size:0.7rem;\">{(newGrade?.IsMissed == true ? "Н" : null)}</p>\r\n";
            return mockup;
        }
        [HttpGet]
        [Authorize(Roles = "teacher")]
        public async Task<string> GetAverage(int studentId, int groupId, int subjectId, int period)
        {
            CertificationPeriod cPeriod = await _context.CertificationPeriods.FindAsync(period);
            if (period == null) return "";
            Student? student = await _context.Students.FindAsync(studentId);
            if (student == null)
            {
                return "";
            }
            int sum = 0;
            int count = 0;
            string res = string.Empty;
            foreach (Grade g in _context.Grades.Where(g => g.User.Id == student.User.Id
            && g.Lesson.LessonInfo.Group.Id == groupId
            && g.Lesson.LessonInfo.Subject.Id == subjectId
            && (g.Lesson.Date >= cPeriod.Start && g.Lesson.Date <= cPeriod.End)
            ))
            {
                if (g.Value != null && g.Value != string.Empty)
                {
                    sum += Convert.ToInt32(g.Value.First().ToString()); count++;
                }
            }
            if (count != 0)
                res = Math.Round(sum / (double)count, 2).ToString();
            return res;
        }
    }
}
