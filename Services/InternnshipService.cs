using System;
using System.Collections.Generic;
using System.Linq;
using SIS.Models;
using SIS.Models.Assignment;
using SIS.Models.Internship;
using SIS.Models.Organization;
using SIS.Models.Period;
using SIS.Repositories;

namespace SIS.Services
{
    public class InternshipService
    {
        private readonly IOrganizationRepository _organizationRepository;
        private readonly IInternshipRepository _internshipRepository;
        private readonly IAssignmentRepository _assignmentRepository;
        private readonly IStudentRepository _studentRepository;
        //private readonly IContactPersonRepository _contactPersonRepository;

        // Constructor for database repositories
        public InternshipService(
            IOrganizationRepository organizationRepository,
            IInternshipRepository internshipRepository,
            IAssignmentRepository assignmentRepository,
            IStudentRepository studentRepository
            //IContactPersonRepository contactPersonRepository
            )
        {
            _organizationRepository = organizationRepository;
            _internshipRepository = internshipRepository;
            _assignmentRepository = assignmentRepository;
            _studentRepository = studentRepository;
            //_contactPersonRepository = contactPersonRepository;
        }

        // Organization Methods
        public bool AddOrganization(Organization organization)
        {
            if (organization == null || !organization.ValidateOrganizationData())
                return false;
                
            return _organizationRepository.Add(organization);
        }

        public List<Organization> GetAllOrganizations()
        {
            return _organizationRepository.GetAll().ToList();
        }

        public Organization GetOrganizationById(int organizationId)
        {
            return _organizationRepository.GetById(organizationId);
        }

        // Internship Methods
        public bool AddInternship(Internship internship)
        {
            if (internship == null || !internship.IsActive)
                return false;
                
            return _internshipRepository.Add(internship);
        }

        public List<Internship> GetInternshipsByOrganizationId(int organizationId)
        {
            return _organizationRepository.GetInternshipsByOrganizationId(organizationId);
        }

        public List<Internship> GetInternshipsByPeriodAndType(Period period, AssignmentType type)
        {
            return _internshipRepository.GetInternshipsByPeriodAndType(period, type);
        }

        public Internship GetInternshipById(int internshipId)
        {
            return _internshipRepository.GetById(internshipId);
        }

        public bool WithdrawInternship(int internshipId)
        {
            return _internshipRepository.WithdrawInternship(internshipId);
        }

        // Assignment Methods
        public bool AddAssignment(Assignment assignment)
        {
            if (assignment == null)
                return false;
                
            // Check if student already has assignment in same period
            if (_assignmentRepository.HasAssignmentInPeriod(
                assignment.Student.StudentNumber, 
                assignment.Internship.Period))
            {
                return false;
            }
                
            return _assignmentRepository.AddAssignment(assignment);
        }

        public bool RemoveAssignment(Assignment assignment)
        {
            return _assignmentRepository.RemoveAssignment(assignment);
        }

        public bool GradeAssignment(int studentNumber, double grade)
        {
            return _assignmentRepository.GradeAssignment(studentNumber, grade);
        }

        // Student Methods
        public bool RegisterStudent(Student student)
        {
            if (student == null || _studentRepository.StudentExists(student.StudentNumber))
                return false;
                
            return _studentRepository.RegisterStudent(student);
        }

        public Student GetStudentByNumber(int studentNumber)
        {
            return _studentRepository.GetByStudentNumber(studentNumber);
        }

        // Contact Person Methods
        // public List<ContactPerson> GetContactPersonsByInternshipId(int internshipId)
        // {
        //     return _internshipRepository.GetContactPersonsByInternshipId(internshipId);
        // }

        // Utility Methods
        
        //comment this for now but I want this in the code later!!!!!
        // public List<Internship> GetAvailableInternshipsForStudent(Student student, Period period)
        // {
        //     return _internshipRepository.GetAvailableInternshipsForStudent(student.StudentNumber, period);
        // }

        public List<Assignment> GetAssignmentsForStudent(int studentNumber)
        {
            return _assignmentRepository.GetAssignmentsForStudent(studentNumber);
        }

        // Additional methods that might be needed
        public List<Internship> GetActiveInternships()
        {
            return _internshipRepository.GetActiveInternships();
        }

        public bool EnrollStudentInInternship(int internshipId, int studentNumber)
        {
            var student = GetStudentByNumber(studentNumber);
            if (student == null) return false;
            
            return _internshipRepository.EnrollStudent(internshipId, student);
        }

        public List<Student> GetEnrolledStudents(int internshipId)
        {
            return _internshipRepository.GetEnrolledStudents(internshipId);
        }

        public List<Internship> SearchInternships(string searchTerm)
        {
            return _internshipRepository.Search(searchTerm);
        }

        public List<Internship> GetInternshipsByCity(string city)
        {
            return _internshipRepository.GetByCity(city);
        }

        public int GetEnrolledStudentCount(int internshipId)
        {
            return _internshipRepository.GetEnrolledStudentCount(internshipId);
        }

        // public bool IsStudentEnrolled(int internshipId, int studentNumber)
        // {
        //     return _internshipRepository.IsStudentEnrolled(internshipId, studentNumber);
        // }

        // Statistics and reporting methods
        public int GetOrganizationCount()
        {
            return _organizationRepository.GetAll().Count();
        }

        public int GetStudentCount()
        {
            return _studentRepository.GetAll().Count();
        }

        public int GetActiveInternshipCount()
        {
            return _internshipRepository.GetActiveInternships().Count;
        }

        public int GetAssignmentCount()
        {
            return _assignmentRepository.GetAll().Count();
        }

        // Search methods
        public List<Organization> SearchOrganizationsByName(string searchTerm)
        {
            return _organizationRepository.SearchByName(searchTerm);
        }

        public List<Student> SearchStudentsByName(string searchTerm)
        {
            return _studentRepository.SearchByName(searchTerm).ToList();
        }

        // Get assignments by various criteria
        public List<Assignment> GetAssignmentsByInternshipId(int internshipId)
        {
            return _assignmentRepository.GetAssignmentsByInternshipId(internshipId);
        }

        public bool UpdateInternship(Internship internship)
        {
            return _internshipRepository.Update(internship);
        }

        public bool UpdateOrganization(Organization organization)
        {
            return _organizationRepository.Update(organization);
        }

        // Delete methods (use with caution)
        public bool DeleteStudent(int studentNumber)
        {
            return _studentRepository.Delete(studentNumber);
        }

        public bool DeleteInternship(int internshipId)
        {
            return _internshipRepository.Delete(internshipId);
        }

        public bool DeleteOrganization(int organizationId)
        {
            return _organizationRepository.Delete(organizationId);
        }

        // Check existence methods
        public bool OrganizationExists(int organizationId)
        {
            return _organizationRepository.Exists(organizationId);
        }

        public bool InternshipExists(int internshipId)
        {
            return _internshipRepository.Exists(internshipId);
        }

        public bool StudentExists(int studentNumber)
        {
            return _studentRepository.StudentExists(studentNumber);
        }
    }
}