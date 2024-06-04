﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace E_Diary.WEB.Areas.Manage.Models
{
    public class ParentViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Установите имя")]
        [DisplayName("Имя")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Установите фамилию")]
        [DisplayName("Фамилия")]
        public string Surname { get; set; }
        [DisplayName("Отчество (при наличии)")]
        public string? Patronymic { get; set; }
        [Required(ErrorMessage = "Выберите пол")]
        [DisplayName("Пол")]
        public int Gender { get; set; }
        [Required(ErrorMessage = "Укажите адрес электронной почты")]
        [DataType(DataType.EmailAddress)]
        [DisplayName("Эл. почта")]
        public string EmailAddress { get; set; }
        [Required(ErrorMessage = "Укажите пароль")]
        [DisplayName("Пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}