using E_Diary.WEB.Data.Entities;
using System.ComponentModel;

namespace E_Diary.WEB.Models
{
    public class GroupViewModel
    {
        public int Id { get; set; }
        [DisplayName("Год обучения")]
        public int Year { get; set; }
        [DisplayName("Буква класса")]
        public char Literal { get; set; }
        [DisplayName("Список учеников")]
        public List<Student> Students { get; set; }
    }
}
