using System;
using SIS.Models.Internship;

namespace SIS.Models.Assignment
{
    public class Assignment
    {
        public Student Student { get; set; }
        public double? FinalGrade { get; private set; }
        public Internship.Internship Internship { get; set; }
        public bool IsCompleted => FinalGrade.HasValue;
        public AssignmentType AssignmentType { get; set; }

        public Assignment(Student student, Internship.Internship internship, AssignmentType assignmentType)
        {
            Student = student ?? throw new ArgumentNullException(nameof(student));
            Internship = internship ?? throw new ArgumentNullException(nameof(internship));
            AssignmentType = assignmentType;
        }

        public bool GradeAssignment(double grade)
        {
            if (grade < 1.0 || grade > 10.0)
                return false;
                
            FinalGrade = Math.Round(grade, 1);
            return true;
        }

        public override string ToString()
        {
            return $"Assignment for {Student.FullName} ({Student.StudentNumber})\n" +
                   $"Internship: {Internship.ProjectTitle}\n" +
                   $"Type: {AssignmentType}\n" +
                   $"Grade: {(IsCompleted ? FinalGrade.ToString() : "Not graded")}\n" +
                   $"Completed: {IsCompleted}";
        }
    }
}