using E_Diary.WEB.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace E_Diary.WEB.Areas.Student.Models
{
    public class GradesViewModel
    {
        public List<Grade> Grades {  get; set; }
        public List<Subject> Subjects { get; set; }
        public StudyYear StudyYear { get; set; }
        public CertificationPeriod CertificationPeriod { get; set; }
    }
}
