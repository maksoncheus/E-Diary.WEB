namespace E_Diary.WEB.Data.Entities
{
    public class Lesson
    {
        public long Id { get; set; }
        public virtual TeacherGroupSubject LessonInfo { get; set; }
        public int LessonOnDayNumber { get; set; }
        public DateOnly Date {  get; set; }
        public string? Title { get; set; }
        public string? HomeWork { get; set; }
    }
}
