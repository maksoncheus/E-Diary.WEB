using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace E_Diary.WEB.Data.Entities
{
    [Index(nameof(Year), nameof(Literal), IsUnique = true)]

    public class Group
    {
        public int Id { get; set; }
        [DisplayName("Год обучения")]
        public int Year { get; set; }
        [DisplayName("Буква класса")]
        public char Literal { get; set; }
    }
}
