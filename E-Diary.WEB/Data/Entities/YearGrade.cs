namespace E_Diary.WEB.Data.Entities
{
    public class YearGrade
    {
        public int Id { get; set; }
        public string? Value { get; set; }
        public virtual TeacherGroupSubject YearInfo { get; set; }
        public virtual StudyYear StudyYear { get; set; }
        public virtual User User { get; set; }
    }
}
