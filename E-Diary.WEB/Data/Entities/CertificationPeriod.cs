using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace E_Diary.WEB.Data.Entities
{
    public class CertificationPeriod
    {
        public int Id { get; set; }
        [DisplayName("Название")]
        [Required(ErrorMessage = "Укажите название")]
        public string Name { get; set; } = null!;
        [DisplayName("Учебный год")]
        [Required(ErrorMessage = "Укажите учебный год")]
        public virtual StudyYear StudyYear { get; set; }
        [DisplayName("Начало")]
        [Required(ErrorMessage = "Укажите начало аттестационного периода")]
        public DateOnly Start { get; set; }
        [DisplayName("Окончание")]
        [Required(ErrorMessage = "Укажите конец аттестационного периода")]
        public DateOnly End { get; set; }
    }
}
