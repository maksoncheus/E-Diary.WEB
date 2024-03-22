using E_Diary.WEB.Data.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel;

namespace E_Diary.WEB.Data.Entities
{
    public class User : IdentityUser
    {
        [DisplayName("Имя")]
        public string Name { get; set; }
        [DisplayName("Отчество")]
        public string? Patronymic { get; set; }
        [DisplayName("Фамилия")]
        public string Surname { get; set; }
        [DisplayName("Пол")]
        public Gender Gender { get; set; }
    }
}
