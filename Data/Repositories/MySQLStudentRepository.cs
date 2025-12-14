using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;
using SIS.Models;
using SIS.Models.Assignment;
using SIS.Repositories;

namespace SIS.Data.Repositories
{
    public class MySQLStudentRepository : DatabaseContext, IStudentRepository
    {
        private readonly IAssignmentRepository _assignmentRepository;
        public MySQLStudentRepository(string connectionString) : base(connectionString) { }

        public bool Add(Student student)
        {
            return RegisterStudent(student);
        }

public bool RegisterStudent(Student student)
{
    if (StudentExists(student.StudentNumber))
        return false;

    try
    {
        // Insert into Person table
        string personSql = @"INSERT INTO Person (First_name, Last_name, PersonType) 
                            VALUES (@FirstName, @LastName, 'Student')";
        
        var personParams = new[]
        {
            new MySqlParameter("@FirstName", student.FirstName),
            new MySqlParameter("@LastName", student.LastName)
        };

        // Use a custom ExecuteNonQuery that returns int
        int personRows = ExecuteNonQueryWithReturn(personSql, personParams);
        
        if (personRows <= 0) return false;

        // Get the auto-generated Person_id
        int personId = Convert.ToInt32(ExecuteScalar("SELECT LAST_INSERT_ID()"));

        // Insert into Student table
        string studentSql = @"INSERT INTO Student (Student_number, Person_id) 
                             VALUES (@StudentNumber, @PersonId)";
        
        var studentParams = new[]
        {
            new MySqlParameter("@StudentNumber", student.StudentNumber),
            new MySqlParameter("@PersonId", personId)
        };

        int studentRows = ExecuteNonQueryWithReturn(studentSql, studentParams);
        
        if (studentRows > 0)
        {
            student.PersonId = personId;  // Now Student has PersonId property
            return true;
        }
        
        return false;
    }
    catch (MySqlException ex)
    {
        Console.WriteLine($"Error registering student: {ex.Message}");
        return false;
    }
}

// Helper method if DatabaseContext.ExecuteNonQuery returns void
private int ExecuteNonQueryWithReturn(string sql, params MySqlParameter[] parameters)
{
    using (var connection = GetConnection())
    using (var command = new MySqlCommand(sql, connection))
    {
        if (parameters != null)
        {
            command.Parameters.AddRange(parameters);
        }
        return command.ExecuteNonQuery();
    }
}

        public bool Update(Student student)
        {
            // Update Person information
            string sql = @"UPDATE Person p
                          INNER JOIN Student s ON p.Person_id = s.Person_id
                          SET p.First_name = @FirstName, p.Last_name = @LastName
                          WHERE s.Student_number = @StudentNumber";
            
            var parameters = new[]
            {
                new MySqlParameter("@FirstName", student.FirstName),
                new MySqlParameter("@LastName", student.LastName),
                new MySqlParameter("@StudentNumber", student.StudentNumber)
            };

            int rowsAffected = 0;
            using (var command = new MySqlCommand(sql, GetConnection()))
            {
                command.Parameters.AddRange(parameters);
                rowsAffected = command.ExecuteNonQuery();
            }
            
            return rowsAffected > 0;
        }

        public bool Delete(int studentNumber) // studentNumber is the ID
        {
            try
            {
                // Get Person_id for this student
                string getPersonIdSql = "SELECT Person_id FROM Student WHERE Student_number = @StudentNumber";
                var personIdObj = ExecuteScalar(getPersonIdSql, new MySqlParameter("@StudentNumber", studentNumber));
                
                if (personIdObj == null) return false;
                
                int personId = Convert.ToInt32(personIdObj);

                // Delete from Student table first (foreign key constraint)
                string deleteStudentSql = "DELETE FROM Student WHERE Student_number = @StudentNumber";
                ExecuteNonQuery(deleteStudentSql, new MySqlParameter("@StudentNumber", studentNumber));

                // Delete from Person table
                string deletePersonSql = "DELETE FROM Person WHERE Person_id = @PersonId";
                ExecuteNonQuery(deletePersonSql, new MySqlParameter("@PersonId", personId));

                return true;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Error deleting student: {ex.Message}");
                return false;
            }
        }

        public Student GetById(int studentNumber) // studentNumber is the ID
        {
            return GetByStudentNumber(studentNumber);
        }

        public Student GetByStudentNumber(int studentNumber)
        {
            string sql = @"SELECT s.Student_number, p.Person_id, p.First_name, p.Last_name
                          FROM Student s
                          INNER JOIN Person p ON s.Person_id = p.Person_id
                          WHERE s.Student_number = @StudentNumber";
            
            using (var reader = ExecuteReader(sql, new MySqlParameter("@StudentNumber", studentNumber)))
            {
                if (reader.Read())
                {
                    return new Student
                    (
                        //PersonId = Convert.ToInt32(reader["Person_id"]),
                        reader["First_name"].ToString(),
                        reader["Last_name"].ToString(),
                        Convert.ToInt32(reader["Student_number"])
                    );
                }
            }
            return null;
        }

        public List<Assignment> GetAssignments(int studentNumber)
        {
            return _assignmentRepository.GetAssignmentsForStudent(studentNumber);
        }

        public double? GetGPA(int studentNumber)
        {
            string sql = @"SELECT AVG(final_grade) as AverageGrade
                          FROM Assignment 
                          WHERE Student_number = @StudentNumber 
                            AND final_grade IS NOT NULL";
            
            var result = ExecuteScalar(sql, new MySqlParameter("@StudentNumber", studentNumber));
            
            if (result == DBNull.Value || result == null)
                return null;
            
            return Convert.ToDouble(result);
        }

         public List<Student> GetStudentsByInternship(int internshipId)
        {
            var students = new List<Student>();
            
            string sql = @"SELECT s.Student_number
                          FROM Assignment a
                          INNER JOIN Student s ON a.Student_number = s.Student_number
                          WHERE a.internship_id = @InternshipId
                          ORDER BY s.Student_number";
            
            using (var reader = ExecuteReader(sql, new MySqlParameter("@InternshipId", internshipId)))
            {
                while (reader.Read())
                {
                    int studentNumber = Convert.ToInt32(reader["Student_number"]);
                    var student = GetByStudentNumber(studentNumber);
                    if (student != null)
                        students.Add(student);
                }
            }
            
            return students;
        }

                public List<Student> GetStudentsWithCompletedAssignments()
        {
            var students = new List<Student>();
            
            string sql = @"SELECT DISTINCT s.Student_number
                          FROM Student s
                          INNER JOIN Assignment a ON s.Student_number = a.Student_number
                          WHERE a.is_completed = true
                          ORDER BY s.Student_number";
            
            using (var reader = ExecuteReader(sql))
            {
                while (reader.Read())
                {
                    int studentNumber = Convert.ToInt32(reader["Student_number"]);
                    var student = GetByStudentNumber(studentNumber);
                    if (student != null)
                        students.Add(student);
                }
            }
            
            return students;
        }

        public List<Student> GetStudentsByAssignmentType(AssignmentType assignmentType)
        {
            var students = new List<Student>();
            
            string sql = @"SELECT DISTINCT s.Student_number
                          FROM Student s
                          INNER JOIN Assignment a ON s.Student_number = a.Student_number
                          WHERE a.assignment_type = @AssignmentType
                          ORDER BY s.Student_number";
            
            var parameter = new MySqlParameter("@AssignmentType", assignmentType.ToString());
            
            using (var reader = ExecuteReader(sql, parameter))
            {
                while (reader.Read())
                {
                    int studentNumber = Convert.ToInt32(reader["Student_number"]);
                    var student = GetByStudentNumber(studentNumber);
                    if (student != null)
                        students.Add(student);
                }
            }
            
            return students;
        }

        public IEnumerable<Student> GetAll()
        {
            var students = new List<Student>();
            
            string sql = @"SELECT s.Student_number, p.Person_id, p.First_name, p.Last_name
                          FROM Student s
                          INNER JOIN Person p ON s.Person_id = p.Person_id
                          ORDER BY s.Student_number";
            
            using (var reader = ExecuteReader(sql))
            {
                while (reader.Read())
                {
                    students.Add(new Student
                    (
                        //PersonId = Convert.ToInt32(reader["Person_id"]),
                        reader["First_name"].ToString(),
                        reader["Last_name"].ToString(),
                        Convert.ToInt32(reader["Student_number"])
                    ));
                }
            }
            
            return students;
        }

        public bool Exists(int studentNumber)
        {
            return StudentExists(studentNumber);
        }


        public bool StudentExists(int studentNumber)
        {
            string sql = "SELECT COUNT(*) FROM Student WHERE Student_number = @StudentNumber";
            var count = Convert.ToInt32(ExecuteScalar(sql, new MySqlParameter("@StudentNumber", studentNumber)));
            return count > 0;
        }

        // Additional methods
        public List<Student> SearchByName(string searchTerm)
        {
            var students = new List<Student>();
            
            string sql = @"SELECT s.Student_number, p.Person_id, p.First_name, p.Last_name
                          FROM Student s
                          INNER JOIN Person p ON s.Person_id = p.Person_id
                          WHERE p.First_name LIKE @SearchTerm OR p.Last_name LIKE @SearchTerm
                          ORDER BY p.Last_name, p.First_name";
            
            var parameter = new MySqlParameter("@SearchTerm", $"%{searchTerm}%");
            
            using (var reader = ExecuteReader(sql, parameter))
            {
                while (reader.Read())
                {
                    students.Add(new Student
                    (
                        //PersonId = Convert.ToInt32(reader["Person_id"]),
                        reader["First_name"].ToString(),
                        reader["Last_name"].ToString(),
                        Convert.ToInt32(reader["Student_number"])
                    ));
                }
            }
            
            return students;
        }

        public int GetStudentCount()
        {
            string sql = "SELECT COUNT(*) FROM Student";
            return Convert.ToInt32(ExecuteScalar(sql));
        }

        // Helper method to get student's assignments count
        public int GetAssignmentCount(int studentNumber)
        {
            string sql = "SELECT COUNT(*) FROM Assignment WHERE Student_number = @StudentNumber";
            return Convert.ToInt32(ExecuteScalar(sql, new MySqlParameter("@StudentNumber", studentNumber)));
        }
    }
}