using E_Diary.WEB.Data.Entities;

namespace E_Diary.WEB.Models
{
    public class ScheduleViewModel
    {
        public int GroupId { get; set; }
        public DateOnly StartOfWeek { get; set; }
        public DateOnly EndOfWeek { get; set; }
        public List<Lesson> Lessons { get; set; }
    }
}
