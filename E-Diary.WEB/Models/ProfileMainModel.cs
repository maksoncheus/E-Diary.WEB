using E_Diary.WEB.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace E_Diary.WEB.Models
{
    public class ProfileMainModel
    {
        [Required(ErrorMessage = "Не указано имя")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Не указана фамилия")]
        public string Surname { get; set; }
        [Required(ErrorMessage = "Не указано отчество")]
        public string Patronymic { get; set; }
        [Required]
        public Gender Gender { get; set; }
        [Required]
        public string OldPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
        [Required]
        [Compare(nameof(NewPassword))]
        public string ConfirmNewPassword { get; set; }
        [Required]
        public string Email { get; set; }

    }
}
