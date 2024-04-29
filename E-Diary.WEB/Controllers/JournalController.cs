using E_Diary.WEB.Data;
using E_Diary.WEB.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace E_Diary.WEB.Controllers
{
    public class JournalController : Controller
    {
        private readonly ASPIdentityDBContext _context;
        public JournalController(ASPIdentityDBContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "teacher")]
        public IActionResult Index(int? groupId, int? subjectId)
        {
            Teacher? teacher = _context.Teachers.FirstOrDefault(
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

            if(tgs == null)
                return NotFound();
            return View(tgs);
        }
        [HttpPost]
        public async Task<IActionResult> RemoveGrade(int studentId, long lessonId)
        {
            Lesson? lesson = await _context.Lessons.FindAsync(lessonId);
            Student? student = await _context.Students.FindAsync(studentId);
            Grade? grade = await _context.Grades.FirstOrDefaultAsync(g => g.Lesson.Id == lesson.Id && g.User.Id == student.User.Id);
            if (grade != null)
            {
                _context.Grades.Remove(grade);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index", new { groupId = lesson.LessonInfo.Group.Id, subjectId = lesson.LessonInfo.Subject.Id });

        }
        [HttpPost]
        public async Task<IActionResult> SetGrade(int studentId, long lessonId)
        {
            bool needToChange = false;
            Lesson? lesson = await _context.Lessons.FindAsync(lessonId);
            Student? student = await _context.Students.FindAsync(studentId);
            if (student == null)
            {
                return NotFound();
            }
            if (lesson == null) return NotFound();
            var form = HttpContext.Request.Form;
            string? grade = form["grade"];
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
            bool isMissed = Convert.ToBoolean(form["isMissed"]);
            newGrade.IsMissed = isMissed;
            if(needToChange)
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
            return RedirectToAction("Index", new { groupId = lesson.LessonInfo.Group.Id, subjectId = lesson.LessonInfo.Subject.Id});
        }
    }
}
