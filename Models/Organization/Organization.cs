using System;
using System.Collections.Generic;
using SIS.Models;

namespace SIS.Models.Organization
{
    public abstract class Organization
    {
        private static int _nextId = 1;
        
        public int OrganizationId { get; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Url { get; set; }
        public ContactPerson ContactPerson { get; set; }
        public List<Internship.Internship> Internships { get; private set; }

        protected Organization(string name, string address, string phone, 
                              string email, string url, ContactPerson contactPerson)
        {
            OrganizationId = _nextId++;
            Name = name;
            Address = address;
            Phone = phone;
            Email = email;
            Url = url;
            ContactPerson = contactPerson;
            Internships = new List<Internship.Internship>();
        }

        public virtual bool ValidateOrganizationData()
        {
            return !string.IsNullOrWhiteSpace(Name) &&
                   !string.IsNullOrWhiteSpace(Address) &&
                   !string.IsNullOrWhiteSpace(Email) &&
                   ContactPerson != null;
        }

        public bool AddInternship(Internship.Internship internship)
        {
            if (internship == null || !internship.IsActive)
                return false;
                
            Internships.Add(internship);
            return true;
        }

        public override bool Equals(object obj)
        {
            if (obj is Organization other)
            {
                return Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase) &&
                       Address.Equals(other.Address, StringComparison.OrdinalIgnoreCase) &&
                       Email.Equals(other.Email, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name.ToLower(), Address.ToLower(), Email.ToLower());
        }
    }
}