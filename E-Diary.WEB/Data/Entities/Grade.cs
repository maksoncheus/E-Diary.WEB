﻿namespace E_Diary.WEB.Data.Entities
{
    public class Grade
    {
        public int Id { get; set; }
        public string? Value { get; set; }
        public virtual Lesson Lesson { get; set; }
        public virtual User User { get; set; }
        public bool IsMissed { get; set; } = false;
    }
}
