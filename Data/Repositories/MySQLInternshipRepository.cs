// MySQLInternshipRepository.cs
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using SIS.Models;
using SIS.Models.Internship;
using SIS.Models.Assignment;
using SIS.Models.Period;
using SIS.Models.Organization;

using SIS.Repositories;

namespace SIS.Data.Repositories
{
    public class MySQLInternshipRepository : DatabaseContext, IInternshipRepository
    {
        private readonly IOrganizationRepository _organizationRepository;
                //private readonly IContactPersonRepository _contactPersonRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IAssignmentRepository _assignmentRepository;
        
        public MySQLInternshipRepository(string connectionString, IOrganizationRepository orgRepo,
                IStudentRepository studentRepo,
            IAssignmentRepository assignmentRepo
        ) 
            : base(connectionString)
        {
            _organizationRepository = orgRepo;
            _studentRepository = studentRepo;
            _assignmentRepository = assignmentRepo;
        }

        public bool Add(Internship internship)
        {
            string sql = @"INSERT INTO Internships 
                          (OrganizationId, City, ProjectTitle, ShortDescription, LongDescription, 
                           Year, Semester, AssignmentType, IsActive, DateOfSubmission) 
                          VALUES (@OrgId, @City, @Title, @ShortDesc, @LongDesc, 
                                  @Year, @Semester, @Type, @IsActive, @DateOfSubmission)";
            
            var parameters = new[]
            {
                new MySqlParameter("@OrgId", internship.Organization.OrganizationId),
                new MySqlParameter("@City", internship.City),
                new MySqlParameter("@Title", internship.ProjectTitle),
                new MySqlParameter("@ShortDesc", internship.ShortDescription),
                new MySqlParameter("@LongDesc", internship.LongDescription ?? (object)DBNull.Value),
                new MySqlParameter("@Year", internship.Period.Year),
                new MySqlParameter("@Semester", internship.Period.Semester.ToString()),
                new MySqlParameter("@Type", internship.AssignmentType.ToString()),
                new MySqlParameter("@IsActive", internship.IsActive),
                new MySqlParameter("@DateOfSubmission", internship.DateOfSubmission)
            };

            ExecuteNonQuery(sql, parameters);
            return true;
        }

        public List<Internship> GetInternshipsByPeriodAndType(Period period, AssignmentType type)
        {
            var internships = new List<Internship>();
            
            string sql = @"SELECT i.*, o.* 
                          FROM Internships i
                          INNER JOIN Organizations o ON i.OrganizationId = o.OrganizationId
                          WHERE i.Year = @Year AND i.Semester = @Semester 
                          AND i.AssignmentType = @Type AND i.IsActive = true
                          ORDER BY i.City, o.Name";
            
            var parameters = new[]
            {
                new MySqlParameter("@Year", period.Year),
                new MySqlParameter("@Semester", period.Semester.ToString()),
                new MySqlParameter("@Type", type.ToString())
            };

            using (var reader = ExecuteReader(sql, parameters))
            {
                while (reader.Read())
                {
                    internships.Add(MapInternshipFromReader(reader));
                }
            }
            
            return internships;
        }

        private Internship MapInternshipFromReader(MySqlDataReader reader)
        {
            Internship internship;
            
            var assignmentType = Enum.Parse<AssignmentType>(reader["AssignmentType"].ToString());
            
            // Fetch the organization
            var organizationId = Convert.ToInt32(reader["OrganizationId"]);
            var organization = _organizationRepository.GetById(organizationId);
            
            // Create the period
            var year = Convert.ToInt32(reader["Year"]);
            var semester = Enum.Parse<Semester>(reader["Semester"].ToString());
            var period = new Period(year, semester);
            
            switch (assignmentType)
            {
                case AssignmentType.MINOR:
                    internship = new MinorInternship(
                        organization as EducationalInstitute,
                        reader["City"].ToString(),
                        Convert.ToInt64(reader["DateOfSubmission"]),
                        reader["ProjectTitle"].ToString(),
                        reader["ShortDescription"].ToString(),
                        reader["LongDescription"] != DBNull.Value ? reader["LongDescription"].ToString() : null,
                        period,
                        Convert.ToInt32(reader["InternshipId"])
                    );
                    break;
                case AssignmentType.ENGINEERING:
                    internship = new GraduationInternship(
                        organization as EducationalInstitute,
                        reader["City"].ToString(),
                        Convert.ToInt64(reader["DateOfSubmission"]),
                        reader["ProjectTitle"].ToString(),
                        reader["ShortDescription"].ToString(),
                        reader["LongDescription"] != DBNull.Value ? reader["LongDescription"].ToString() : null,
                        period
                    );
                    break;
                case AssignmentType.RESEARCH:
                    internship = new IntermediateInternship(
                        organization as EducationalInstitute,
                        reader["City"].ToString(),
                        Convert.ToInt64(reader["DateOfSubmission"]),
                        reader["ProjectTitle"].ToString(),
                        reader["ShortDescription"].ToString(),
                        reader["LongDescription"] != DBNull.Value ? reader["LongDescription"].ToString() : null,
                        period
                    );
                    break;
                default:
                    throw new InvalidOperationException($"Unknown assignment type: {assignmentType}");
            }

            internship.IsActive = Convert.ToBoolean(reader["IsActive"]);
            
            return internship;
        }

        public Internship GetById(int internshipId)
        {
            // Your existing implementation
            string sql = @"SELECT i.*, o.*
                          FROM Internship i
                          INNER JOIN Organization o ON i.organization_id = o.organization_id
                          WHERE i.internship_id = @InternshipId";
            
            using (var reader = ExecuteReader(sql, new MySqlParameter("@InternshipId", internshipId)))
            {
                if (reader.Read())
                {
                    return MapInternshipFromReader(reader);
                }
            }
            return null;
        }

        public IEnumerable<Internship> GetAll()
        {
            // Your existing implementation
            var internships = new List<Internship>();
            
            string sql = @"SELECT i.*, o.*
                          FROM Internship i
                          INNER JOIN Organization o ON i.organization_id = o.organization_id
                          ORDER BY i.title";
            
            using (var reader = ExecuteReader(sql))
            {
                while (reader.Read())
                {
                    internships.Add(MapInternshipFromReader(reader));
                }
            }
            
            return internships;
        }

        public bool Exists(int internshipId)
        {
            string sql = "SELECT COUNT(*) FROM Internship WHERE internship_id = @InternshipId";
            var count = Convert.ToInt32(ExecuteScalar(sql, new MySqlParameter("@InternshipId", internshipId)));
            return count > 0;
        }

        // Now implementing the requested methods:

        public bool WithdrawInternship(int internshipId)
        {
            try
            {
                string sql = "UPDATE Internship SET is_active = false WHERE internship_id = @InternshipId";
                int rowsAffected = 0;
                using (var command = new MySqlCommand(sql, GetConnection()))
                {
                    command.Parameters.Add(new MySqlParameter("@InternshipId", internshipId));
                    rowsAffected = command.ExecuteNonQuery();
                }
                
                return rowsAffected > 0;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Error withdrawing internship: {ex.Message}");
                return false;
            }
        }

        public bool EnrollStudent(int internshipId, Student student)
        {
            // First check if student is already enrolled
            if (IsStudentEnrolled(internshipId, student.StudentNumber))
            {
                Console.WriteLine($"Student {student.StudentNumber} is already enrolled in internship {internshipId}");
                return false;
            }

            // Create an assignment for this enrollment
            var internship = GetById(internshipId);
            if (internship == null)
            {
                Console.WriteLine($"Internship {internshipId} not found");
                return false;
            }

            var assignment = new Assignment(student, internship, internship.AssignmentType)
            {
                Student = student,
                Internship = internship,
                AssignmentType = internship.AssignmentType,
                //IsCompleted = false,
                //FinalGrade = null
            };

            return _assignmentRepository.AddAssignment(assignment);
        }

        // public bool WithdrawStudent(int internshipId, Student student)
        // {
        //     try
        //     {
        //         // Find the assignment for this student and internship
        //         var assignment = _assignmentRepository.GetByStudentAndInternship(student.StudentNumber, internshipId);
        //         if (assignment == null)
        //         {
        //             Console.WriteLine($"No assignment found for student {student.StudentNumber} in internship {internshipId}");
        //             return false;
        //         }

        //         // Delete the assignment (which removes the enrollment)
        //         return _assignmentRepository.Delete(assignment.AssignmentId);
        //     }
        //     catch (MySqlException ex)
        //     {
        //         Console.WriteLine($"Error withdrawing student: {ex.Message}");
        //         return false;
        //     }
        // }

        // public bool AddContactPersonToInternship(int internshipId, int contactPersonId)
        // {
        //     return _contactPersonRepository.AddContactPersonToInternship(contactPersonId, internshipId);
        // }

        public List<Student> GetEnrolledStudents(int internshipId)
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
                    var student = _studentRepository.GetByStudentNumber(studentNumber);
                    if (student != null)
                        students.Add(student);
                }
            }
            
            return students;
        }

        public List<Internship> Search(string searchTerm)
        {
            var internships = new List<Internship>();
            
            string sql = @"SELECT i.*, o.*
                          FROM Internship i
                          INNER JOIN Organization o ON i.organization_id = o.organization_id
                          WHERE i.title LIKE @SearchTerm 
                             OR i.short_description LIKE @SearchTerm
                             OR i.long_description LIKE @SearchTerm
                             OR i.city LIKE @SearchTerm
                          ORDER BY i.title";
            
            var parameter = new MySqlParameter("@SearchTerm", $"%{searchTerm}%");
            
            using (var reader = ExecuteReader(sql, parameter))
            {
                while (reader.Read())
                {
                    internships.Add(MapInternshipFromReader(reader));
                }
            }
            
            return internships;
        }

        public List<Internship> GetByCity(string city)
        {
            var internships = new List<Internship>();
            
            string sql = @"SELECT i.*, o.*
                          FROM Internship i
                          INNER JOIN Organization o ON i.organization_id = o.organization_id
                          WHERE i.city = @City
                          ORDER BY i.title";
            
            var parameter = new MySqlParameter("@City", city);
            
            using (var reader = ExecuteReader(sql, parameter))
            {
                while (reader.Read())
                {
                    internships.Add(MapInternshipFromReader(reader));
                }
            }
            
            return internships;
        }

        public List<Internship> GetActiveInternships()
        {
            var internships = new List<Internship>();
            
            string sql = @"SELECT i.*, o.*
                          FROM Internship i
                          INNER JOIN Organization o ON i.organization_id = o.organization_id
                          WHERE i.is_active = true
                          ORDER BY i.title";
            
            using (var reader = ExecuteReader(sql))
            {
                while (reader.Read())
                {
                    internships.Add(MapInternshipFromReader(reader));
                }
            }
            
            return internships;
        }

        public int GetEnrolledStudentCount(int internshipId)
        {
            string sql = "SELECT COUNT(*) FROM Assignment WHERE internship_id = @InternshipId";
            var count = Convert.ToInt32(ExecuteScalar(sql, new MySqlParameter("@InternshipId", internshipId)));
            return count;
        }

        // Additional methods from IInternshipRepository interface:

        public List<Internship> GetAvailableInternshipsForStudent(int studentNumber, Period period)
        {
            var internships = new List<Internship>();
            
            string sql = @"SELECT i.*, o.*
                          FROM Internship i
                          INNER JOIN Organization o ON i.organization_id = o.organization_id
                          WHERE i.year = @Year 
                            AND i.semester = @Semester
                            AND i.is_active = true
                            AND i.internship_id NOT IN (
                                SELECT internship_id 
                                FROM Assignment 
                                WHERE Student_number = @StudentNumber
                            )
                          ORDER BY i.title";
            
            var parameters = new[]
            {
                new MySqlParameter("@Year", period.Year),
                new MySqlParameter("@Semester", period.Semester.ToString()),
                new MySqlParameter("@StudentNumber", studentNumber)
            };

            using (var reader = ExecuteReader(sql, parameters))
            {
                while (reader.Read())
                {
                    internships.Add(MapInternshipFromReader(reader));
                }
            }
            
            return internships;
        }

        public List<Internship> GetByOrganizationAndPeriod(int organizationId, Period period)
        {
            var internships = new List<Internship>();
            
            string sql = @"SELECT i.*, o.*
                          FROM Internship i
                          INNER JOIN Organization o ON i.organization_id = o.organization_id
                          WHERE i.organization_id = @OrganizationId
                            AND i.year = @Year 
                            AND i.semester = @Semester
                            AND i.is_active = true
                          ORDER BY i.title";
            
            var parameters = new[]
            {
                new MySqlParameter("@OrganizationId", organizationId),
                new MySqlParameter("@Year", period.Year),
                new MySqlParameter("@Semester", period.Semester.ToString())
            };

            using (var reader = ExecuteReader(sql, parameters))
            {
                while (reader.Read())
                {
                    internships.Add(MapInternshipFromReader(reader));
                }
            }
            
            return internships;
        }

        public bool IsStudentEnrolled(int internshipId, int studentNumber)
        {
            string sql = @"SELECT COUNT(*) 
                          FROM Assignment a
                          WHERE a.internship_id = @InternshipId 
                            AND a.Student_number = @StudentNumber";
            
            var parameters = new[]
            {
                new MySqlParameter("@InternshipId", internshipId),
                new MySqlParameter("@StudentNumber", studentNumber)
            };

            var count = Convert.ToInt32(ExecuteScalar(sql, parameters));
            return count > 0;
        }

        

        // Helper method for mapping
        // private Internship MapInternshipFromReader(MySqlDataReader reader)
        // {
        //     var internshipType = reader["internship_type"].ToString();
        //     var assignmentType = Enum.Parse<AssignmentType>(reader["assignment_type"].ToString());

        //     Internship internship = internshipType switch
        //     {
        //         "Minor" => new MinorInternship(),
        //         "Graduation" => new GraduationInternship(),
        //         "Intermediate" => new IntermediateInternship(),
        //         _ => new Internship()
        //     };

        //     internship.InternshipId = Convert.ToInt32(reader["internship_id"]);
        //     internship.ProjectTitle = reader["title"].ToString();
        //     internship.ShortDescription = reader["short_description"].ToString();
        //     internship.LongDescription = reader["long_description"] != DBNull.Value 
        //         ? reader["long_description"].ToString() 
        //         : null;
        //     internship.City = reader["city"].ToString();
        //     internship.DateOfSubmission = Convert.ToInt64(reader["date_of_submission"]);
        //     internship.IsActive = Convert.ToBoolean(reader["is_active"]);
        //     internship.AssignmentType = assignmentType;

        //     internship.Period = new Period
        //     {
        //         Year = Convert.ToInt32(reader["year"]),
        //         Semester = Enum.Parse<Semester>(reader["semester"].ToString())
        //     };

        //     var organization = _organizationRepository.MapOrganizationFromReader(reader);
        //     internship.Organization = organization;

        //     var contactPersons = GetContactPersonsByInternshipId(internship.InternshipId);
        //     internship.ContactPersons = contactPersons;

        //     return internship;
        // }

        // Additional interface methods that might be needed:


        public List<Internship> GetInternshipsWithAvailableSlots()
        {
            // Assuming each internship has a maximum capacity
            // You might need to add a capacity field to your database
            var allInternships = GetActiveInternships();
            return allInternships.Where(i => GetEnrolledStudentCount(i.InternshipId) < 10).ToList(); // Example: max 10 students
        }

        public List<Internship> GetByAssignmentType(AssignmentType assignmentType)
        {
            var internships = new List<Internship>();
            
            string sql = @"SELECT i.*, o.*
                          FROM Internship i
                          INNER JOIN Organization o ON i.organization_id = o.organization_id
                          WHERE i.assignment_type = @AssignmentType
                            AND i.is_active = true
                          ORDER BY i.title";
            
            var parameter = new MySqlParameter("@AssignmentType", assignmentType.ToString());
            
            using (var reader = ExecuteReader(sql, parameter))
            {
                while (reader.Read())
                {
                    internships.Add(MapInternshipFromReader(reader));
                }
            }
            
            return internships;
        }

        public bool Update(Internship internship)
{
    try
    {
        string internshipType = internship switch
        {
            MinorInternship => "Minor",
            GraduationInternship => "Graduation",
            IntermediateInternship => "Intermediate",
            _ => "General"
        };

        string sql = @"UPDATE Internship 
                      SET title = @Title,
                          short_description = @ShortDesc,
                          long_description = @LongDesc,
                          city = @City,
                          organization_id = @OrgId,
                          internship_type = @InternshipType,
                          year = @Year,
                          semester = @Semester,
                          date_of_submission = @DateOfSubmission,
                          is_active = @IsActive,
                          assignment_type = @AssignmentType
                      WHERE internship_id = @InternshipId";
        
        var parameters = new[]
        {
            new MySqlParameter("@Title", internship.ProjectTitle),
            new MySqlParameter("@ShortDesc", internship.ShortDescription),
            new MySqlParameter("@LongDesc", internship.LongDescription ?? (object)DBNull.Value),
            new MySqlParameter("@City", internship.City),
            new MySqlParameter("@OrgId", internship.Organization.OrganizationId),
            new MySqlParameter("@InternshipType", internshipType),
            new MySqlParameter("@Year", internship.Period.Year),
            new MySqlParameter("@Semester", internship.Period.Semester.ToString()),
            new MySqlParameter("@DateOfSubmission", internship.DateOfSubmission),
            new MySqlParameter("@IsActive", internship.IsActive),
            new MySqlParameter("@AssignmentType", internship.AssignmentType.ToString()),
            new MySqlParameter("@InternshipId", internship.InternshipId)
        };

        int rowsAffected = 0;
        using (var command = new MySqlCommand(sql, GetConnection()))
        {
            command.Parameters.AddRange(parameters);
            rowsAffected = command.ExecuteNonQuery();
        }
        
        return rowsAffected > 0;
    }
    catch (MySqlException ex)
    {
        Console.WriteLine($"Error updating internship: {ex.Message}");
        return false;
    }
}

    public bool Delete(int internshipId)
{
    try
    {
        // Check if internship has any assignments
        string checkAssignmentsSql = "SELECT COUNT(*) FROM Assignment WHERE internship_id = @InternshipId";
        var assignmentCount = Convert.ToInt32(ExecuteScalar(checkAssignmentsSql, 
            new MySqlParameter("@InternshipId", internshipId)));
        
        if (assignmentCount > 0)
        {
            Console.WriteLine("Cannot delete internship with existing assignments.");
            return false;
        }

        // Remove contact person associations first
        string removeCpiSql = "DELETE FROM contact_person_internship WHERE internship_id = @InternshipId";
        ExecuteNonQuery(removeCpiSql, new MySqlParameter("@InternshipId", internshipId));

        // Delete internship
        string sql = "DELETE FROM Internship WHERE internship_id = @InternshipId";
        int rowsAffected = 0;
        using (var command = new MySqlCommand(sql, GetConnection()))
        {
            command.Parameters.Add(new MySqlParameter("@InternshipId", internshipId));
            rowsAffected = command.ExecuteNonQuery();
        }
        
        return rowsAffected > 0;
    }
    catch (MySqlException ex)
    {
        Console.WriteLine($"Error deleting internship: {ex.Message}");
        return false;
    }
}
    }
}