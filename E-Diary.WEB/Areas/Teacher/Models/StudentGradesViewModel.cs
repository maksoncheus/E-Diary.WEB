using E_Diary.WEB.Data.Entities;

namespace E_Diary.WEB.Areas.Teacher.Models
{
    public class StudentGradesViewModel
    {
        public List<Grade> Grades { get; set; }
        public List<Subject> Subjects { get; set; }
        public StudyYear StudyYear { get; set; }
        public CertificationPeriod CertificationPeriod { get; set; }
    }
}
