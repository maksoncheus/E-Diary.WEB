namespace E_Diary.WEB.Data.Entities
{
    public class CertificationPeriod
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public virtual StudyYear StudyYear { get; set; }
        public DateOnly Start { get; set; }
        public DateOnly End { get; set; }
    }
}
