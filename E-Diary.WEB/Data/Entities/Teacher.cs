namespace E_Diary.WEB.Data.Entities
{
    public class Teacher
    {
        public int Id { get; set; }
        public virtual User User { get; set; }
    }
}
