namespace E_Diary.WEB.Data.Entities
{
    public class PeriodGrade
    {
        public int Id { get; set; }
        public int Value { get; set; }
        public virtual TeacherGroupSubject PeriodInfo { get; set; }
        public virtual CertificationPeriod CertificationPeriod { get; set; }
        public virtual User User { get; set; }
    }
}
