using SIS.Models.Assignment;
using SIS.Models.Period;
using System.Collections.Generic;

namespace SIS.Repositories
{
    /// <summary>
    /// Repository interface for Assignment entities
    /// </summary>
    public interface IAssignmentRepository : IRepository<Assignment>
    {
        /// <summary>
        /// Adds a new assignment (specific method for Assignment entities)
        /// </summary>
        /// <param name="assignment">Assignment to add</param>
        /// <returns>True if successful</returns>
        bool AddAssignment(Assignment assignment);

        /// <summary>
        /// Removes an assignment
        /// </summary>
        /// <param name="assignment">Assignment to remove</param>
        /// <returns>True if successful</returns>
        bool RemoveAssignment(Assignment assignment);

        /// <summary>
        /// Grades an assignment for a student
        /// </summary>
        /// <param name="studentNumber">Student number</param>
        /// <param name="grade">Grade to assign</param>
        /// <returns>True if successful</returns>
        bool GradeAssignment(int studentNumber, double grade);

        /// <summary>
        /// Gets all assignments for a specific student
        /// </summary>
        /// <param name="studentNumber">Student number</param>
        /// <returns>List of assignments</returns>
        List<Assignment> GetAssignmentsForStudent(int studentNumber);

        /// <summary>
        /// Checks if a student has an assignment in a specific period
        /// </summary>
        /// <param name="studentNumber">Student number</param>
        /// <param name="period">Period to check</param>
        /// <returns>True if has assignment in period</returns>
        bool HasAssignmentInPeriod(int studentNumber, Period period);

        /// <summary>
        /// Marks an assignment as completed
        /// </summary>
        /// <param name="assignmentId">Assignment ID</param>
        /// <returns>True if successful</returns>
        bool CompleteAssignment(int assignmentId);

        /// <summary>
        /// Gets all assignments for a specific internship
        /// </summary>
        /// <param name="internshipId">Internship ID</param>
        /// <returns>List of assignments</returns>
        List<Assignment> GetAssignmentsByInternshipId(int internshipId);

        /// <summary>
        /// Gets assignment by student and internship
        /// </summary>
        /// <param name="studentNumber">Student number</param>
        /// <param name="internshipId">Internship ID</param>
        /// <returns>Assignment or null if not found</returns>
        //Assignment GetByStudentAndInternship(int studentNumber, int internshipId);

        /// <summary>
        /// Gets assignments by assignment type
        /// </summary>
        /// <param name="assignmentType">Type of assignment</param>
        /// <returns>List of assignments</returns>
        //List<Assignment> GetByAssignmentType(AssignmentType assignmentType);

        /// <summary>
        /// Gets assignments by completion status
        /// </summary>
        /// <param name="isCompleted">Completion status</param>
        /// <returns>List of assignments</returns>
        //List<Assignment> GetByCompletionStatus(bool isCompleted);

        /// <summary>
        /// Gets assignments with grades above a certain threshold
        /// </summary>
        /// <param name="minGrade">Minimum grade</param>
        /// <returns>List of assignments</returns>
        //List<Assignment> GetAssignmentsAboveGrade(double minGrade);

        /// <summary>
        /// Gets the average grade for assignments of a specific type
        /// </summary>
        /// <param name="assignmentType">Assignment type</param>
        /// <returns>Average grade</returns>
        double GetAverageGradeByType(AssignmentType assignmentType);

        /// <summary>
        /// Updates the final grade for an assignment
        /// </summary>
        /// <param name="assignmentId">Assignment ID</param>
        /// <param name="grade">New grade</param>
        /// <returns>True if successful</returns>
        bool UpdateGrade(int assignmentId, double grade);

        /// <summary>
        /// Gets assignments for a student that are not yet completed
        /// </summary>
        /// <param name="studentNumber">Student number</param>
        /// <returns>List of pending assignments</returns>
        //List<Assignment> GetPendingAssignmentsForStudent(int studentNumber);
    }
}