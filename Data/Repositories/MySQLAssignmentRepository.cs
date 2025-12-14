using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;
using SIS.Models;
using SIS.Models.Assignment;
using SIS.Models.Internship;
using SIS.Models.Period;
using SIS.Repositories;

namespace SIS.Data.Repositories
{
    public class MySQLAssignmentRepository : DatabaseContext, IAssignmentRepository
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IInternshipRepository _internshipRepository;
        
        public MySQLAssignmentRepository(string connectionString, 
            IStudentRepository studentRepo) 
            : base(connectionString)
        {
            _studentRepository = studentRepo;
            //internshipRepository = internshipRepo;
        }

        public bool Add(Assignment assignment)
        {
            return AddAssignment(assignment);
        }

        public bool AddAssignment(Assignment assignment)
        {
            string sql = @"INSERT INTO Assignment 
                          (internship_id, assignment_type, final_grade, Student_number, is_completed) 
                          VALUES (@InternshipId, @AssignmentType, @FinalGrade, @StudentNumber, @IsCompleted)";
            
            var parameters = new[]
            {
                new MySqlParameter("@InternshipId", assignment.Internship.InternshipId),
                new MySqlParameter("@AssignmentType", assignment.AssignmentType.ToString()),
                new MySqlParameter("@FinalGrade", assignment.FinalGrade.HasValue ? 
                    (object)assignment.FinalGrade.Value : DBNull.Value),
                new MySqlParameter("@StudentNumber", assignment.Student.StudentNumber),
                new MySqlParameter("@IsCompleted", assignment.IsCompleted)
            };

            ExecuteNonQuery(sql, parameters);

            // Get the inserted assignment ID
           // assignment.AssignmentId = Convert.ToInt32(ExecuteScalar("SELECT LAST_INSERT_ID()"));
            
            return true;
        }

        public bool Update(Assignment assignment)
        {
            string sql = @"UPDATE Assignment 
                          SET internship_id = @InternshipId,
                              assignment_type = @AssignmentType,
                              final_grade = @FinalGrade,
                              Student_number = @StudentNumber,
                              is_completed = @IsCompleted
                          WHERE Assignment_id = @AssignmentId";
            
            var parameters = new[]
            {
                //new MySqlParameter("@AssignmentId", assignment.AssignmentId),
                new MySqlParameter("@InternshipId", assignment.Internship.InternshipId),
                new MySqlParameter("@AssignmentType", assignment.AssignmentType.ToString()),
                new MySqlParameter("@FinalGrade", assignment.FinalGrade.HasValue ? 
                    (object)assignment.FinalGrade.Value : DBNull.Value),
                new MySqlParameter("@StudentNumber", assignment.Student.StudentNumber),
                new MySqlParameter("@IsCompleted", assignment.IsCompleted)
            };

            ExecuteNonQuery(sql, parameters);
            return true;
        }

        public bool Delete(int id)
        {
            string sql = "DELETE FROM Assignment WHERE Assignment_id = @Id";
            var parameter = new MySqlParameter("@Id", id);
            
            int rowsAffected = 0;
            using (var command = new MySqlCommand(sql, GetConnection()))
            {
                command.Parameters.Add(parameter);
                rowsAffected = command.ExecuteNonQuery();
            }
            
            return rowsAffected > 0;
        }

        public bool RemoveAssignment(Assignment assignment)
        {
            if (assignment == null) return false;
            
            return Delete(assignment);
        }

        private bool Delete(Assignment assignment)
        {
            throw new NotImplementedException();
        }

        public Assignment GetById(int id)
        {
            string sql = @"SELECT a.*, s.*, i.*, p.*
                          FROM Assignment a
                          INNER JOIN Student s ON a.Student_number = s.Student_number
                          INNER JOIN Person ps ON s.Person_id = ps.Person_id
                          INNER JOIN Internship i ON a.internship_id = i.internship_id
                          INNER JOIN Period p ON i.year = p.year AND i.semester = p.semester
                          WHERE a.Assignment_id = @Id";
            
            var parameter = new MySqlParameter("@Id", id);
            
            using (var reader = ExecuteReader(sql, parameter))
            {
                if (reader.Read())
                {
                    return MapAssignmentFromReader(reader);
                }
            }
            return null;
        }

        public IEnumerable<Assignment> GetAll()
        {
            var assignments = new List<Assignment>();
            
            string sql = @"SELECT a.*, s.*, i.*, p.*
                          FROM Assignment a
                          INNER JOIN Student s ON a.Student_number = s.Student_number
                          INNER JOIN Person ps ON s.Person_id = ps.Person_id
                          INNER JOIN Internship i ON a.internship_id = i.internship_id
                          INNER JOIN Period p ON i.year = p.year AND i.semester = p.semester
                          ORDER BY a.Assignment_id";
            
            using (var reader = ExecuteReader(sql))
            {
                while (reader.Read())
                {
                    assignments.Add(MapAssignmentFromReader(reader));
                }
            }
            
            return assignments;
        }

        public bool Exists(int id)
        {
            string sql = "SELECT COUNT(*) FROM Assignment WHERE Assignment_id = @Id";
            var parameter = new MySqlParameter("@Id", id);
            
            var count = Convert.ToInt32(ExecuteScalar(sql, parameter));
            return count > 0;
        }

        private Assignment MapAssignmentFromReader(MySqlDataReader reader)
        {
            var student = new Student
            (
                //PersonId = Convert.ToInt32(reader["Person_id"]),
                reader["First_name"].ToString(),
                reader["Last_name"].ToString(),
                Convert.ToInt32(reader["Student_number"])
            );

            var internship = _internshipRepository.GetById(Convert.ToInt32(reader["internship_id"]));

            var assignment = new Assignment
            (
                //AssignmentId = Convert.ToInt32(reader["Assignment_id"]),
                student,
                internship,
                Enum.Parse<AssignmentType>(reader["assignment_type"].ToString())
                // IsCompleted = Convert.ToBoolean(reader["is_completed"])
            );

            if (reader["final_grade"] != DBNull.Value)
            {
                assignment.FinalGrade = Convert.ToDouble(reader["final_grade"]);
            }

            return assignment;
        }

        public bool GradeAssignment(int studentNumber, double grade)
        {
            string sql = @"UPDATE Assignment 
                          SET final_grade = @Grade, 
                              is_completed = CASE WHEN @Grade >= 5.5 THEN true ELSE false END
                          WHERE Student_number = @StudentNumber";
            
            var parameters = new[]
            {
                new MySqlParameter("@Grade", grade),
                new MySqlParameter("@StudentNumber", studentNumber)
            };

            ExecuteNonQuery(sql, parameters);
            return true;
        }

        public List<Assignment> GetAssignmentsForStudent(int studentNumber)
        {
            var assignments = new List<Assignment>();
            
            string sql = @"SELECT a.*, s.*, i.*, p.*
                          FROM Assignment a
                          INNER JOIN Student s ON a.Student_number = s.Student_number
                          INNER JOIN Person ps ON s.Person_id = ps.Person_id
                          INNER JOIN Internship i ON a.internship_id = i.internship_id
                          INNER JOIN Period p ON i.year = p.year AND i.semester = p.semester
                          WHERE a.Student_number = @StudentNumber
                          ORDER BY a.Assignment_id";
            
            var parameter = new MySqlParameter("@StudentNumber", studentNumber);
            
            using (var reader = ExecuteReader(sql, parameter))
            {
                while (reader.Read())
                {
                    assignments.Add(MapAssignmentFromReader(reader));
                }
            }
            
            return assignments;
        }

        public bool HasAssignmentInPeriod(int studentNumber, Period period)
        {
            string sql = @"SELECT COUNT(*) 
                          FROM Assignment a
                          INNER JOIN Internship i ON a.internship_id = i.internship_id
                          WHERE a.Student_number = @StudentNumber
                          AND i.year = @Year
                          AND i.semester = @Semester";
            
            var parameters = new[]
            {
                new MySqlParameter("@StudentNumber", studentNumber),
                new MySqlParameter("@Year", period.Year),
                new MySqlParameter("@Semester", period.Semester.ToString())
            };

            var count = Convert.ToInt32(ExecuteScalar(sql, parameters));
            return count > 0;
        }

        public bool CompleteAssignment(int assignmentId)
        {
            string sql = "UPDATE Assignment SET is_completed = true WHERE Assignment_id = @Id";
            var parameter = new MySqlParameter("@Id", assignmentId);
            
            ExecuteNonQuery(sql, parameter);
            return true;
        }

        public List<Assignment> GetAssignmentsByInternshipId(int internshipId)
        {
            var assignments = new List<Assignment>();
            
            string sql = @"SELECT a.*, s.*, i.*, p.*
                          FROM Assignment a
                          INNER JOIN Student s ON a.Student_number = s.Student_number
                          INNER JOIN Person ps ON s.Person_id = ps.Person_id
                          INNER JOIN Internship i ON a.internship_id = i.internship_id
                          INNER JOIN Period p ON i.year = p.year AND i.semester = p.semester
                          WHERE a.internship_id = @InternshipId
                          ORDER BY a.Assignment_id";
            
            var parameter = new MySqlParameter("@InternshipId", internshipId);
            
            using (var reader = ExecuteReader(sql, parameter))
            {
                while (reader.Read())
                {
                    assignments.Add(MapAssignmentFromReader(reader));
                }
            }
            
            return assignments;
        }

        public double GetAverageGradeByType(AssignmentType assignmentType)
        {
            string sql = @"SELECT AVG(final_grade) 
                          FROM Assignment 
                          WHERE assignment_type = @AssignmentType 
                            AND final_grade IS NOT NULL";
            
            var parameter = new MySqlParameter("@AssignmentType", assignmentType.ToString());
            
            var result = ExecuteScalar(sql, parameter);
            if (result == DBNull.Value || result == null)
                return 0.0;
            
            return Convert.ToDouble(result);
        }

        public bool UpdateGrade(int assignmentId, double grade)
        {
            try
            {
                string sql = @"UPDATE Assignment 
                              SET final_grade = @Grade, 
                                  is_completed = CASE WHEN @Grade >= 5.5 THEN true ELSE false END
                              WHERE Assignment_id = @AssignmentId";
                
                var parameters = new[]
                {
                    new MySqlParameter("@Grade", grade),
                    new MySqlParameter("@AssignmentId", assignmentId)
                };

                ExecuteNonQuery(sql, parameters);
                return true;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Error updating grade: {ex.Message}");
                return false;
            }
        }
    }
}