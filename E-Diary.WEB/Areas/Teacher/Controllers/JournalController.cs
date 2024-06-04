using E_Diary.WEB.Areas.Teacher.Models;
using E_Diary.WEB.Data;
using E_Diary.WEB.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace E_Diary.WEB.Areas.Teacher.Controllers
{
    /// <summary>
    /// Контроллер "Журнал" для преподавателя
    /// </summary>
    [Area("Teacher")]
    public class JournalController : Controller
    {
        public const string selectedGradePageConst = "border-1 border-dark bg-white border-start border-end border-top ";
        private readonly ASPIdentityDBContext _context;
        public JournalController(ASPIdentityDBContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "teacher")]
public async Task<string> GetLessonInfo(long lessonId)
        {
            Lesson? lesson = await _context.Lessons.FindAsync(lessonId);
            if (lesson == null) return string.Empty;

            return $"<input type=\"hidden\" name=\"lessonId\" value=\"{lesson.Id}\"/>\r\n" +
                $"<div class=\"form-group\">\r\n" +
                $"<label for=\"title\" class=\"control-label\">Тема урока</label>\r\n" +
                $"<input id=\"title\" type=\"text\" name=\"title\" value=\"{lesson.Title ?? ""}\" class=\"form-control\"/>\r\n" +
                $"</div>\r\n" +
                $"<div class=\"form-group\">\r\n" +
                $"<label for=\"homeWork\" class=\"control-label\">Домашнее задание</label>\r\n" +
                $"<textarea id=\"homeWork\" name=\"homeWork\" class=\"form-control\">{lesson.HomeWork ?? ""}</textarea>\r\n" +
                $"</div>";
        }
        [Authorize(Roles = "teacher")]
        public async Task<IActionResult> ChangeLessonInfo(long lessonId, string title, string homeWork)
        {
            Lesson? lesson = await _context.Lessons.FindAsync(lessonId);
            if (lesson == null) return BadRequest();
            lesson.Title = title;
            lesson.HomeWork = homeWork;
            _context.Entry(lesson).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok();
        }
        /// <summary>
        /// Основная страница преподавателя.
        /// </summary>
        /// <param name="date">Дата, на которую требуется отобразить расписание</param>
        [Authorize(Roles = "teacher")]
        public IActionResult Main(string? date)
        {
            DateOnly dateOnly = date != null ? DateOnly.Parse(date) : DateOnly.FromDateTime(DateTime.Now);
            return View(dateOnly);
        }
        /// <summary>
        /// Страница "Оценки". Здесь ведется основная работа преподавателя с эл. журналом - выставление оценок.
        /// </summary>
        /// <param name="groupId">ID группы, для которой требуется отобразить оценки</param>
        /// <param name="subjectId">ID предмета, для которого требуется отобразить оценки</param>
        /// <param name="periodId">ID аттестационного периода, на который требуется отобразить оценки</param>
        /// <returns></returns>
        [Authorize(Roles = "teacher")]
        public IActionResult Grades(int? groupId, int? subjectId, int periodId = -1)
        {
            Data.Entities.Teacher? teacher = _context.Teachers.FirstOrDefault(
                t => t.User.UserName == HttpContext.User.Identity.Name);
            if (teacher == null)
                return Forbid();
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);
            StudyYear? year = _context.StudyYears.OrderBy(y => y.End).LastOrDefault(y => y.Start <= today);
            if(year == null) return NotFound();
            IEnumerable<TeacherGroupSubject> tgss = _context.TeacherGroupSubjects
                .Where(tgs => tgs.Teacher.Id == teacher.Id && tgs.StudyYear.Id == year.Id);

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
            //Если аттестационный период не указан в запросе, открывается последний на текущую дату
            if (periodId == -1)
            {
                certificationPeriodId = _context.CertificationPeriods.OrderBy(p => p.End).LastOrDefault(p => p.Start <= today)?.Id ?? -1;
            }
            else certificationPeriodId = periodId;
            if (certificationPeriodId == -1)
                return NotFound();
            GradesViewModel model = new()
            {
                TeacherGroupSubjectId = tgs.Id,
                CertificationPeriodId = certificationPeriodId
            };
            return View(model);
        }
        [Authorize(Roles = "teacher")]
        public IActionResult AnnualGrades(int? groupId, int? subjectId)
        {
            Data.Entities.Teacher? teacher = _context.Teachers.FirstOrDefault(
                t => t.User.UserName == HttpContext.User.Identity.Name);
            if (teacher == null)
                return Forbid();
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);
            StudyYear? year = _context.StudyYears.OrderBy(y => y.End).LastOrDefault(y => y.Start <= today);
            if(year == null) return NotFound();
            IEnumerable<TeacherGroupSubject> tgss = _context.TeacherGroupSubjects
                .Where(tgs => tgs.Teacher.Id == teacher.Id && tgs.StudyYear.Id == year.Id);

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
            AnnualGradesViewModel model = new()
            {
                TeacherGroupSubjectId = tgs.Id,
                StudyYearId = year.Id
            };
            return View(model);
        }
        /// <summary>
        /// Удалить оценку у определенного ученика на определенном уроке
        /// </summary>
        /// <param name="studentId">ID ученика</param>
        /// <param name="lessonId">ID урока</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "teacher")]
        public async Task<string> RemoveGrade(int studentId, long lessonId)
        {
            Lesson? lesson = await _context.Lessons.FindAsync(lessonId);
            if (lesson == null) return "";
            Data.Entities.Student? student = await _context.Students.FindAsync(studentId);
            if (student == null) return "";
            Grade? grade = await _context.Grades.FirstOrDefaultAsync(g => g.Lesson.Id == lesson.Id && g.User.Id == student.User.Id);
            if (grade != null)
            {
                _context.Grades.Remove(grade);
                await _context.SaveChangesAsync();
            }
            //Вовзращаем разметку (данный метод вызывается из клиента с помощью AJAX).
            return "<span class=\"badge badge-grey\"></span>\r\n<p class=\"text-danger m-0 badge-missed\" style=\"position:absolute; top:70%; left:5%; font-size:0.7rem;\">\r\n</p>";

        }
        /// <summary>
        /// Установить оценку
        /// </summary>
        /// <param name="studentId">ID ученика</param>
        /// <param name="lessonId">ID урока</param>
        /// <param name="grade">Оценка</param>
        /// <param name="isMissed">Отметка "Пропуск"</param>
        [HttpPost]
        [Authorize(Roles = "teacher")]
        public async Task<string> SetGrade(int studentId, long lessonId, string grade, bool isMissed)
        {
            bool needToChange = false;
            Lesson? lesson = await _context.Lessons.FindAsync(lessonId);
            Data.Entities.Student? student = await _context.Students.FindAsync(studentId);
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
            //Вовзращаем разметку (данный метод вызывается из клиента с помощью AJAX).
            string mockup = $"<span class=\"badge badge-grey\">{newGrade?.Value ?? ""}</span>\r\n<p class=\"text-danger m-0 badge-missed\" style=\"position:absolute; top:70%; left:5%; font-size:0.7rem;\">{(newGrade?.IsMissed == true ? "Н" : null)}</p>\r\n";
            return mockup;
        }
        /// <summary>
        /// Получить среднюю оценку у ученика
        /// </summary>
        /// <param name="studentId">ID ученика</param>
        /// <param name="groupId">ID группы</param>
        /// <param name="subjectId">ID предмета</param>
        /// <param name="period">Аттестационный период</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "teacher")]
        public async Task<string> GetAverage(int studentId, int groupId, int subjectId, int period)
        {
            CertificationPeriod cPeriod = await _context.CertificationPeriods.FindAsync(period);
            if (period == null) return "";
            Data.Entities.Student? student = await _context.Students.FindAsync(studentId);
            if (student == null)
            {
                return "";
            }
            int sum = 0;
            int count = 0;
            string res = string.Empty;
            //Максимально ограничиваем выборку для ускорения работы
            foreach (Grade g in _context.Grades.Where(g => g.Lesson.LessonInfo.Subject.Id == subjectId
            && g.Lesson.LessonInfo.Group.Id == groupId
            && g.User.Id == student.User.Id
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
        
        [HttpPost]
        [Authorize(Roles = "teacher")]
        public async Task<string> SetCertificationPeriodGrade(int studentId, int tgs, int certificationPeriodId, string grade)
        {
            bool needToChange = false;
            CertificationPeriod? period = await _context.CertificationPeriods.FindAsync(certificationPeriodId);
            TeacherGroupSubject? teacherGroupSubject = await _context.TeacherGroupSubjects.FindAsync(tgs);
            Data.Entities.Student? student = await _context.Students.FindAsync(studentId);
            if (student == null)
            {
                return "";
            }
            if (period == null) return "";
            PeriodGrade? newGrade = await _context.PeriodGrades.FirstOrDefaultAsync(g => g.CertificationPeriod.Id == period.Id && g.User.Id == student.User.Id && g.PeriodInfo.Id == tgs);
            if (newGrade == null)
                newGrade = new();
            else needToChange = true;
            if (grade == null)
            {
                newGrade.Value = null;
            }
            else
                newGrade.Value = grade;
            if (needToChange)
            {
                _context.Entry(newGrade).State = EntityState.Modified;
            }
            else
            {
                newGrade.CertificationPeriod = period;
                newGrade.User = student.User;
                newGrade.PeriodInfo = teacherGroupSubject;
                _context.PeriodGrades.Add(newGrade);
            }
            await _context.SaveChangesAsync();
            //Вовзращаем разметку (данный метод вызывается из клиента с помощью AJAX).
            string mockup = $"<span class=\"badge badge-grey\">{newGrade?.Value ?? ""}</span>";
            return mockup;
        }
        [HttpPost]
        [Authorize(Roles = "teacher")]
        public async Task<string> RemoveCertificationPeriodGrade(int studentId, int tgs, int certificationPeriodId)
        {
            CertificationPeriod? period = await _context.CertificationPeriods.FindAsync(certificationPeriodId);
            TeacherGroupSubject? teacherGroupSubject = await _context.TeacherGroupSubjects.FindAsync(tgs);
            if (period == null) return "";
            Data.Entities.Student? student = await _context.Students.FindAsync(studentId);
            if (student == null) return "";
            PeriodGrade? grade = await _context.PeriodGrades.FirstOrDefaultAsync(g => g.CertificationPeriod.Id == period.Id && g.User.Id == student.User.Id && g.PeriodInfo.Id == teacherGroupSubject.Id);
            if (grade != null)
            {
                _context.PeriodGrades.Remove(grade);
                await _context.SaveChangesAsync();
            }
            //Вовзращаем разметку (данный метод вызывается из клиента с помощью AJAX).
            return "<span class=\"badge badge-grey\"></span>\r\n<p class=\"text-danger m-0 badge-missed\" style=\"position:absolute; top:70%; left:5%; font-size:0.7rem;\">\r\n</p>";
        }
        [HttpPost]
        [Authorize(Roles = "teacher")]
        public async Task<string> SetAnnualGrade(int studentId, int tgs, int yearId, string grade)
        {
            bool needToChange = false;
            StudyYear? year = await _context.StudyYears.FindAsync(yearId);
            Data.Entities.Student? student = await _context.Students.FindAsync(studentId);
            TeacherGroupSubject? teacherGroupSubject = await _context.TeacherGroupSubjects.FindAsync(tgs);
            if (student == null)
            {
                return "";
            }
            if (year == null) return "";
            YearGrade? newGrade = await _context.YearGrades.FirstOrDefaultAsync(g => g.StudyYear.Id == year.Id && g.User.Id == student.User.Id && g.YearInfo.Id == teacherGroupSubject.Id);
            if (newGrade == null)
                newGrade = new();
            else needToChange = true;
            if (grade == null)
            {
                newGrade.Value = null;
            }
            else
                newGrade.Value = grade;
            if (needToChange)
            {
                _context.Entry(newGrade).State = EntityState.Modified;
            }
            else
            {
                newGrade.StudyYear = year;
                newGrade.User = student.User;
                newGrade.YearInfo = teacherGroupSubject;
                _context.YearGrades.Add(newGrade);
            }
            await _context.SaveChangesAsync();
            //Вовзращаем разметку (данный метод вызывается из клиента с помощью AJAX).
            string mockup = $"<span class=\"badge badge-grey\">{newGrade?.Value ?? ""}</span>";
            return mockup;
        }
        [HttpPost]
        [Authorize(Roles = "teacher")]
        public async Task<string> RemoveAnnualPeriodGrade(int studentId, int tgs, int yearId)
        {
            StudyYear? year = await _context.StudyYears.FindAsync(yearId);
            if (year == null) return "";
            Data.Entities.Student? student = await _context.Students.FindAsync(studentId);
            if (student == null) return "";
            YearGrade? grade = await _context.YearGrades.FirstOrDefaultAsync(g => g.StudyYear.Id == year.Id && g.User.Id == student.User.Id && g.YearInfo.Id == tgs);
            if (grade != null)
            {
                _context.YearGrades.Remove(grade);
                await _context.SaveChangesAsync();
            }
            //Вовзращаем разметку (данный метод вызывается из клиента с помощью AJAX).
            return "<span class=\"badge badge-grey\"></span>\r\n<p class=\"text-danger m-0 badge-missed\" style=\"position:absolute; top:70%; left:5%; font-size:0.7rem;\">\r\n</p>";
        }
    }
}
