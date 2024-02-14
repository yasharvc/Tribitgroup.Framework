namespace Test.API.Authentication.Models
{
    public class TimeZone
    {
        public int Hour { get; set; }
        public int Minute { get; set; }
        public TimeZone() { }
        public TimeZone(int hour, int minute) { Hour = hour; Minute = minute; }
        public TimeZone(int hour) : this(hour, 0) { }

        public DateTime ToGMT(DateTime dateTime) { return dateTime.AddHours(Hour).AddMinutes(Minute); }
        public DateTime FromGMT(DateTime dateTime) { return dateTime.AddHours(-Hour).AddMinutes(-Minute); }
    }
}
