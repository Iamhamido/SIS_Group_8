using SIS.Models;
using SIS.Models.Internship;
using SIS.Models.Period;
using SIS.Models.Assignment;
using System.Collections.Generic;

namespace SIS.Repositories
{
    /// <summary>
    /// Repository interface for Internship entities
    /// </summary>
    public interface IInternshipRepository : IRepository<Internship>
    {
        /// <summary>
        /// Gets internships by period and type
        /// </summary>
        /// <param name="period">Period filter</param>
        /// <param name="type">Assignment type filter</param>
        /// <returns>List of matching internships</returns>
        List<Internship> GetInternshipsByPeriodAndType(Period period, AssignmentType type);

        /// <summary>
        /// Gets available internships for a student in a specific period
        /// </summary>
        /// <param name="student">Student</param>
        /// <param name="period">Period</param>
        /// <returns>List of available internships</returns>
        //List<Internship> GetAvailableInternshipsForStudent(Student student, Period period);

        /// <summary>
        /// Withdraws/Deactivates an internship
        /// </summary>
        /// <param name="internshipId">Internship ID</param>
        /// <returns>True if successful</returns>
        bool WithdrawInternship(int internshipId);

        /// <summary>
        /// Enrolls a student in an internship
        /// </summary>
        /// <param name="internshipId">Internship ID</param>
        /// <param name="student">Student to enroll</param>
        /// <returns>True if successful</returns>
        bool EnrollStudent(int internshipId, Student student);

        /// <summary>
        /// Withdraws a student from an internship
        /// </summary>
        /// <param name="internshipId">Internship ID</param>
        /// <param name="student">Student to withdraw</param>
        /// <returns>True if successful</returns>
        //bool WithdrawStudent(int internshipId, Student student);

        /// <summary>
        /// Gets all contact persons for an internship
        /// </summary>
        /// <param name="internshipId">Internship ID</param>
        /// <returns>List of contact persons</returns>
       // List<ContactPerson> GetContactPersonsByInternshipId(int internshipId);

        /// <summary>
        /// Adds a contact person to an internship
        /// </summary>
        /// <param name="internshipId">Internship ID</param>
        /// <param name="contactPerson">Contact person to add</param>
        /// <returns>True if successful</returns>
        //bool AddContactPersonToInternship(int internshipId, ContactPerson contactPerson);

        /// <summary>
        /// Gets all enrolled students for an internship
        /// </summary>
        /// <param name="internshipId">Internship ID</param>
        /// <returns>List of enrolled students</returns>
        List<Student> GetEnrolledStudents(int internshipId);

        /// <summary>
        /// Searches internships by title or description
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <returns>List of matching internships</returns>
        List<Internship> Search(string searchTerm);

        /// <summary>
        /// Gets internships by city
        /// </summary>
        /// <param name="city">City name</param>
        /// <returns>List of internships in the city</returns>
        List<Internship> GetByCity(string city);

        /// <summary>
        /// Gets active internships only
        /// </summary>
        /// <returns>List of active internships</returns>
        List<Internship> GetActiveInternships();

        /// <summary>
        /// Gets the count of enrolled students for an internship
        /// </summary>
        /// <param name="internshipId">Internship ID</param>
        /// <returns>Number of enrolled students</returns>
        int GetEnrolledStudentCount(int internshipId);
    }
}