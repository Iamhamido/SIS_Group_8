using System;
using System.Collections.Generic;
using System.Linq;
using SIS.Models;
using SIS.Models.Assignment;
using SIS.Models.Internship;
using SIS.Models.Organization;
using SIS.Models.Period;

namespace SIS.Services
{
    public class InternshipService
    {
        private List<Organization> _organizations;
        private List<Internship> _internships;
        private List<Assignment> _assignments;
        private List<Student> _students;

        public InternshipService()
        {
            _organizations = new List<Organization>();
            _internships = new List<Internship>();
            _assignments = new List<Assignment>();
            _students = new List<Student>();
        }

        // Organization Methods
        public bool AddOrganization(Organization organization)
        {
            if (organization == null || !organization.ValidateOrganizationData())
                return false;
                
            if (_organizations.Any(o => o.Equals(organization)))
                return false;
                
            _organizations.Add(organization);
            return true;
        }

        public List<Organization> GetAllOrganizations()
        {
            return _organizations
                .OrderBy(o => o.Name)
                .ToList();
        }

        // Internship Methods
        public bool AddInternship(Internship internship)
        {
            if (internship == null || !internship.IsActive)
                return false;
                
            _internships.Add(internship);
            
            // Link to organization
            internship.Organization?.AddInternship(internship);
            
            return true;
        }

        public List<Internship> GetInternshipsByOrganizationId(int organizationId)
        {
            return _internships
                .Where(i => i.Organization?.OrganizationId == organizationId && i.IsActive)
                .OrderBy(i => i.City)
                .ThenBy(i => i.Organization?.Name)
                .ToList();
        }

        public List<Internship> GetInternshipsByPeriodAndType(Period period, AssignmentType type)
        {
            return _internships
                .Where(i => i.Period.Equals(period) && 
                           i.AssignmentType == type && 
                           i.IsActive)
                .OrderBy(i => i.City)
                .ThenBy(i => i.Organization?.Name)
                .ToList();
        }

        public Internship GetInternshipById(int internshipId)
        {
            return _internships.FirstOrDefault(i => i.InternshipId == internshipId);
        }

        public bool WithdrawInternship(int internshipId)
        {
            var internship = GetInternshipById(internshipId);
            if (internship == null)
                return false;
                
            internship.IsActive = false;
            
            // Remove all assignments for this internship
            _assignments.RemoveAll(a => a.Internship.InternshipId == internshipId);
            
            return true;
        }

        // Assignment Methods
        public bool AddAssignment(Assignment assignment)
        {
            if (assignment == null)
                return false;
                
            // Check if student already has assignment in same period
            var studentAssignments = _assignments
                .Where(a => a.Student.StudentNumber == assignment.Student.StudentNumber)
                .ToList();
                
            if (studentAssignments.Any(a => a.Internship.Period.Equals(assignment.Internship.Period)))
                return false;
                
            _assignments.Add(assignment);
            assignment.Internship.EnrollStudent(assignment.Student);
            
            return true;
        }

        public bool RemoveAssignment(Assignment assignment)
        {
            if (assignment == null)
                return false;
                
            var result = _assignments.Remove(assignment);
            if (result)
            {
                assignment.Internship.WithdrawStudent(assignment.Student);
            }
            
            return result;
        }

        public bool GradeAssignment(Student student, double grade)
        {
            var assignment = _assignments.FirstOrDefault(a => a.Student.StudentNumber == student.StudentNumber);
            if (assignment == null)
                return false;
                
            return assignment.GradeAssignment(grade);
        }

        // Student Methods
        public bool RegisterStudent(Student student)
        {
            if (student == null || _students.Any(s => s.StudentNumber == student.StudentNumber))
                return false;
                
            _students.Add(student);
            return true;
        }

        public Student GetStudentByNumber(int studentNumber)
        {
            return _students.FirstOrDefault(s => s.StudentNumber == studentNumber);
        }

        // Contact Person Methods
        public List<ContactPerson> GetContactPersonsByInternshipId(int internshipId)
        {
            var internship = GetInternshipById(internshipId);
            if (internship == null)
                return new List<ContactPerson>();
                
            return internship.ContactPersons
                .OrderBy(cp => cp.LastName)
                .ThenBy(cp => cp.FirstName)
                .ToList();
        }

        // Utility Methods
        public List<Internship> GetAvailableInternshipsForStudent(Student student, Period period)
        {
            // Get internships where student is not enrolled and is active
            return _internships
                .Where(i => i.IsActive && 
                           i.Period.Equals(period) &&
                           !i.EnrolledStudents.Any(s => s.StudentNumber == student.StudentNumber))
                .OrderBy(i => i.City)
                .ThenBy(i => i.Organization?.Name)
                .ToList();
        }

        public List<Assignment> GetAssignmentsForStudent(int studentNumber)
        {
            return _assignments
                .Where(a => a.Student.StudentNumber == studentNumber)
                .ToList();
        }
    }
}