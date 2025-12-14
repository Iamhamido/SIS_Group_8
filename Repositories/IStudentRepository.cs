using SIS.Models;
using SIS.Models.Assignment;
using System.Collections.Generic;

namespace SIS.Repositories
{
    /// <summary>
    /// Repository interface for Student entities
    /// </summary>
    public interface IStudentRepository : IRepository<Student>
    {
        /// <summary>
        /// Gets student by student number
        /// </summary>
        /// <param name="studentNumber">Student number</param>
        /// <returns>Student or null if not found</returns>
        Student GetByStudentNumber(int studentNumber);

        /// <summary>
        /// Registers a new student
        /// </summary>
        /// <param name="student">Student to register</param>
        /// <returns>True if successful</returns>
        bool RegisterStudent(Student student);


        /// <summary>
        /// Gets all assignments for a student
        /// </summary>
        /// <param name="studentNumber">Student number</param>
        /// <returns>List of assignments</returns>
        List<Assignment> GetAssignments(int studentNumber);

        /// <summary>
        /// Gets student's GPA (Grade Point Average)
        /// </summary>
        /// <param name="studentNumber">Student number</param>
        /// <returns>GPA or null if no grades</returns>
        double? GetGPA(int studentNumber);

        /// <summary>
        /// Searches students by name
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <returns>List of matching students</returns>
        List<Student> SearchByName(string searchTerm);

        /// <summary>
        /// Gets students enrolled in a specific internship
        /// </summary>
        /// <param name="internshipId">Internship ID</param>
        /// <returns>List of students</returns>
        List<Student> GetStudentsByInternship(int internshipId);

        /// <summary>
        /// Checks if a student with the given student number exists
        /// </summary>
        /// <param name="studentNumber">Student number to check</param>
        /// <returns>True if exists</returns>
        bool StudentExists(int studentNumber);

        /// <summary>
        /// Gets students with completed assignments
        /// </summary>
        /// <returns>List of students with completed assignments</returns>
        List<Student> GetStudentsWithCompletedAssignments();

        /// <summary>
        /// Gets students without any assignments
        /// </summary>
        /// <returns>List of students without assignments</returns>
        //List<Student> GetStudentsWithoutAssignments();

        /// <summary>
        /// Gets the count of students enrolled in the system
        /// </summary>
        /// <returns>Number of students</returns>
        int GetStudentCount();

        /// <summary>
        /// Updates student information
        /// </summary>
        /// <param name="studentNumber">Student number</param>
        /// <param name="firstName">New first name</param>
        /// <param name="lastName">New last name</param>
        /// <returns>True if successful</returns>
        //bool UpdateStudentInfo(int studentNumber, string firstName, string lastName);

        /// <summary>
        /// Gets students by assignment type they're enrolled in
        /// </summary>
        /// <param name="assignmentType">Type of assignment</param>
        /// <returns>List of students</returns>
        List<Student> GetStudentsByAssignmentType(AssignmentType assignmentType);
    }
}