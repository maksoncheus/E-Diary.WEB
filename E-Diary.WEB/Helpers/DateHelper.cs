namespace E_Diary.WEB.Helpers
{
    public static class DateHelper
    {
        public static DateOnly StartOfWeek(this DateOnly dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff);
        }
        public static DateOnly EndOfWeek(this DateOnly dt, DayOfWeek startOfWeek)
        {
            return dt.StartOfWeek(startOfWeek).AddDays(6);
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff);
        }
    }
}
