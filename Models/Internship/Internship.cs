using System;
using System.Collections.Generic;
using System.Linq;
using SIS.Models.Period;

namespace SIS.Models.Internship
{
    public abstract class Internship
    {
        private static int _nextId = 1;
        
        public int InternshipId { get; }
        public Organization.Organization Organization { get; set; }
        public string City { get; set; }
        public long DateOfSubmission { get; set; }
        public string ProjectTitle { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public Period.Period Period { get; set; }
        public List<Student> EnrolledStudents { get; private set; }
        public List<ContactPerson> ContactPersons { get; private set; }
        public bool IsActive { get; set; }
        public abstract Assignment.AssignmentType AssignmentType { get; }

        protected Internship(Organization.Organization organization, string city, 
                           long dateOfSubmission, string projectTitle, 
                           string shortDescription, string longDescription, Period.Period period)
        {
            InternshipId = _nextId++;
            Organization = organization;
            City = city;
            DateOfSubmission = dateOfSubmission;
            ProjectTitle = projectTitle;
            ShortDescription = shortDescription;
            LongDescription = longDescription;
            Period = period;
            EnrolledStudents = new List<Student>();
            ContactPersons = new List<ContactPerson>();
            IsActive = true;
        }

        public string GetDetails()
        {
            return $"Internship ID: {InternshipId}\n" +
                   $"Organization: {Organization?.Name}\n" +
                   $"City: {City}\n" +
                   $"Project Title: {ProjectTitle}\n" +
                   $"Short Description: {ShortDescription}\n" +
                   $"Long Description: {LongDescription}\n" +
                   $"Period: {Period}\n" +
                   $"Type: {AssignmentType}\n" +
                   $"Active: {IsActive}\n" +
                   $"Enrolled Students: {EnrolledStudents.Count}\n" +
                   $"Contact Persons: {string.Join(", ", ContactPersons.Select(cp => cp.FullName))}";
        }

        public bool AddContactPerson(ContactPerson contactPerson)
        {
            if (contactPerson == null || ContactPersons.Contains(contactPerson))
                return false;
                
            ContactPersons.Add(contactPerson);
            return true;
        }

        public bool EnrollStudent(Student student)
        {
            if (student == null || EnrolledStudents.Contains(student))
                return false;
                
            EnrolledStudents.Add(student);
            return true;
        }

        public bool WithdrawStudent(Student student)
        {
            return EnrolledStudents.Remove(student);
        }
    }
}