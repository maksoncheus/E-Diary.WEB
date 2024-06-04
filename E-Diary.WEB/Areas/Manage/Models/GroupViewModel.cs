using E_Diary.WEB.Data.Entities;
using System.ComponentModel;

namespace E_Diary.WEB.Areas.Manage.Models
{
    public class GroupViewModel
    {
        public int Id { get; set; }
        [DisplayName("Год обучения")]
        public int Year { get; set; }
        [DisplayName("Буква класса")]
        public char Literal { get; set; }
        [DisplayName("Список учеников")]
        public List<Data.Entities.Student>? Students { get; set; }
        [DisplayName("Классный руководитель")]
        public int ClassroomTeacherId { get; set; }
    }
}
