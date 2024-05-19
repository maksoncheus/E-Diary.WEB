using E_Diary.WEB.Data.Entities;

namespace E_Diary.WEB.Areas.Manage.Models
{
    public class CreateCertificationPeriodViewModel
    {
        public string Name { get; set; } = null!;
        public int StudyYearId { get; set; }
        public DateOnly Start { get; set; }
        public DateOnly End { get; set; }
    }
}
