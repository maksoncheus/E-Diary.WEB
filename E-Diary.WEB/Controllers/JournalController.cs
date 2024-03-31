using E_Diary.WEB.Data;
using E_Diary.WEB.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    }
}
