using System;

namespace SIS.Models
{
    public abstract class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        protected Person(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }

        public string FullName => $"{FirstName} {LastName}";
    }
}