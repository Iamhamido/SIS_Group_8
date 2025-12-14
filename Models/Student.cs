namespace SIS.Models
{
    public class Student : Person
    {
        public int StudentNumber { get; set; }
        public int PersonId {get; set;}

        public Student(string firstName, string lastName, int studentNumber) 
            : base(firstName, lastName)
        {
            StudentNumber = studentNumber;
        }
    }
}