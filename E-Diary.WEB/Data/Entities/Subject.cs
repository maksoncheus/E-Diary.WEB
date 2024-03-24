using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace E_Diary.WEB.Data.Entities
{
    public class Subject
    {
        public int Id { get; set; }
        [DisplayName("Название предмета")]
        [Required]
        public string Name { get; set; }
    }
}
