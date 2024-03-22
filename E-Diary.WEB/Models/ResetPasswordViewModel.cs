using System.ComponentModel.DataAnnotations;

namespace E_Diary.WEB.Models
{
    public class ResetPasswordViewModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Token { get; set; }
    }
}
