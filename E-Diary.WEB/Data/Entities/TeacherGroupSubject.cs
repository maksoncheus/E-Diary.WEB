using System.ComponentModel;

namespace E_Diary.WEB.Data.Entities
{
    public class TeacherGroupSubject
    {
        public int Id { get; set; }
        [DisplayName("Учитель")]
        public virtual Teacher Teacher { get; set; }
        [DisplayName("Класс")]
        
        public virtual Group Group { get; set; }
        [DisplayName("Предмет")]
        public virtual Subject Subject { get; set; }
    }
}
