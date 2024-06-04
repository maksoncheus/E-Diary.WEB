using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace E_Diary.WEB.Data.Entities
{
    public class StudyYear
    {
        public int Id { get; set; }
        [DisplayName("Начало")]
        [Required(ErrorMessage = "Укажите начало учебного года")]
        public DateOnly Start { get; set; }
        [Required(ErrorMessage = "Укажите конец учебного года")]
        [DisplayName("Конец")]
        public DateOnly End { get; set; }
    }
}
