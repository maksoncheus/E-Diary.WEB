namespace E_Diary.WEB.Areas.Manage.Models
{
    public class EditCertificationPeriodViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int StudyYearId { get; set; }
        public DateOnly Start { get; set; }
        public DateOnly End { get; set; }
    }
}
