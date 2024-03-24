using E_Diary.WEB.Data.Entities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace E_Diary.WEB.Models
{
    public class TeacherGroupSubjectViewModel
    {
        public int Id { get; set; }
        [DisplayName("Учитель")]
        [Required]
        public int TeacherId { get; set; }
        [DisplayName("Класс")]
        [Required]
        public int GroupId { get; set; }
        [DisplayName("Предмет")]
        [Required]
        public int SubjectId { get; set; }
    }
}
