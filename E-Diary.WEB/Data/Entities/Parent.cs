using System.ComponentModel;

namespace E_Diary.WEB.Data.Entities
{
    public class Parent
    {
        public int Id { get; set; }
        public virtual User User { get; set; }
        [DisplayName("Дети")]
        public virtual ICollection<Student> Children { get; set; }
    }
}
