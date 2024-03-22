using System.ComponentModel;

namespace E_Diary.WEB.Data.Entities
{
    public class Student
    {
        public int Id { get; set; }
        [DisplayName("Класс")]
        public virtual Group Group { get; set; }
        public virtual User User { get; set; }
    }
}
