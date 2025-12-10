namespace SIS.Models
{
    public class ContactPerson : Person
    {
        public string FunctionTitle { get; set; }
        public string Department { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        public ContactPerson(string firstName, string lastName, string functionTitle, 
                           string department, string phone, string email)
            : base(firstName, lastName)
        {
            FunctionTitle = functionTitle;
            Department = department;
            Phone = phone;
            Email = email;
        }
    }
}