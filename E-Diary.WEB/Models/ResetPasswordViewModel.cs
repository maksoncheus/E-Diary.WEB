using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace E_Diary.WEB.Models
{
    public class ResetPasswordViewModel
    {
        [Required]
        public string Id { get; set; }
        [DisplayName("Новый пароль")]
        [Required(ErrorMessage = "Укажите новый пароль")]
        public string Password { get; set; }
        [DisplayName("Подтвердите пароль")]
        [Required(ErrorMessage = "Подтвердите пароль")]
        [Compare(nameof(Password), ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }
        [Required]
        public string Token { get; set; }
    }
}
