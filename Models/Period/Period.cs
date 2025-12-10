namespace SIS.Models.Period
{
    public class Period
    {
        public int Year { get; set; }
        public Semester Semester { get; set; }

        public Period(int year, Semester semester)
        {
            Year = year;
            Semester = semester;
        }

        public override string ToString() => $"{Year}-{Semester}";

        public override bool Equals(object obj)
        {
            if (obj is Period other)
            {
                return Year == other.Year && Semester == other.Semester;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Year, Semester);
        }
    }
}