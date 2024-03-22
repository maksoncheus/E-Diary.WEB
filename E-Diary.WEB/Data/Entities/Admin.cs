using System.ComponentModel;

namespace E_Diary.WEB.Data.Entities
{
    public class Admin
    {
        public int Id { get; set; }
        [DisplayName("Должность")]
        public string Position { get; set; }
        public virtual User User { get; set; }
    }
}
