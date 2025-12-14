using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;
using SIS.Models;
using SIS.Models.Internship;
using SIS.Models.Organization;
using SIS.Models.Period;
using SIS.Repositories;

namespace SIS.Data.Repositories
{
    public class MySQLOrganizationRepository : DatabaseContext, IOrganizationRepository
    {
        public MySQLOrganizationRepository(string connectionString) : base(connectionString) { }

        public bool Add(Organization organization)
        {
            try
            {
                // Determine organization type
                string orgType = organization switch
                {
                    Company => "Company",
                    ResearchGroup => "ResearchGroup",
                    EducationalInstitute => "EducationalInstitute",
                    _ => "Organization"
                };

                string instituteType = null;
                if (organization is EducationalInstitute educationalInstitute)
                {
                    instituteType = educationalInstitute.InstituteType.ToString();
                }

                string sql = @"INSERT INTO Organization 
                              (name, phone_number, url, address, organization_type, email, institute_type) 
                              VALUES (@Name, @Phone, @Url, @Address, @OrgType, @Email, @InstituteType)";
                
                var parameters = new[]
                {
                    new MySqlParameter("@Name", organization.Name),
                    new MySqlParameter("@Phone", organization.Phone),
                    new MySqlParameter("@Url", organization.Url ?? (object)DBNull.Value),
                    new MySqlParameter("@Address", organization.Address),
                    new MySqlParameter("@OrgType", orgType),
                    new MySqlParameter("@Email", organization.Email ?? (object)DBNull.Value),
                    new MySqlParameter("@InstituteType", instituteType ?? (object)DBNull.Value)
                };

                ExecuteNonQuery(sql, parameters);

                // Get the auto-generated organization_id
                organization.OrganizationId = Convert.ToInt32(ExecuteScalar("SELECT LAST_INSERT_ID()"));
                
                return true;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Error adding organization: {ex.Message}");
                return false;
            }
        }

        public bool Update(Organization organization)
        {
            try
            {
                string orgType = organization switch
                {
                    Company => "Company",
                    ResearchGroup => "ResearchGroup",
                    EducationalInstitute => "EducationalInstitute",
                    _ => "Organization"
                };

                string instituteType = null;
                if (organization is EducationalInstitute educationalInstitute)
                {
                    instituteType = educationalInstitute.InstituteType.ToString();
                }

                string sql = @"UPDATE Organization 
                              SET name = @Name,
                                  phone_number = @Phone,
                                  url = @Url,
                                  address = @Address,
                                  organization_type = @OrgType,
                                  email = @Email,
                                  institute_type = @InstituteType
                              WHERE organization_id = @OrganizationId";
                
                var parameters = new[]
                {
                    new MySqlParameter("@Name", organization.Name),
                    new MySqlParameter("@Phone", organization.Phone),
                    new MySqlParameter("@Url", organization.Url ?? (object)DBNull.Value),
                    new MySqlParameter("@Address", organization.Address),
                    new MySqlParameter("@OrgType", orgType),
                    new MySqlParameter("@Email", organization.Email ?? (object)DBNull.Value),
                    new MySqlParameter("@InstituteType", instituteType ?? (object)DBNull.Value),
                    new MySqlParameter("@OrganizationId", organization.OrganizationId)
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
                Console.WriteLine($"Error updating organization: {ex.Message}");
                return false;
            }
        }

        public bool Delete(int organizationId)
        {
            try
            {
                // Check if organization has any internships
                string checkInternshipsSql = "SELECT COUNT(*) FROM Internship WHERE organization_id = @OrganizationId";
                var internshipCount = Convert.ToInt32(ExecuteScalar(checkInternshipsSql, 
                    new MySqlParameter("@OrganizationId", organizationId)));
                
                if (internshipCount > 0)
                {
                    Console.WriteLine("Cannot delete organization with existing internships.");
                    return false;
                }

                // Delete organization
                string sql = "DELETE FROM Organization WHERE organization_id = @OrganizationId";
                int rowsAffected = 0;
                using (var command = new MySqlCommand(sql, GetConnection()))
                {
                    command.Parameters.Add(new MySqlParameter("@OrganizationId", organizationId));
                    rowsAffected = command.ExecuteNonQuery();
                }
                
                return rowsAffected > 0;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Error deleting organization: {ex.Message}");
                return false;
            }
        }

        public Organization GetById(int organizationId)
        {
            string sql = "SELECT * FROM Organization WHERE organization_id = @OrganizationId";
            
            using (var reader = ExecuteReader(sql, new MySqlParameter("@OrganizationId", organizationId)))
            {
                if (reader.Read())
                {
                    return MapOrganizationFromReader(reader);
                }
            }
            return null;
        }

        public Organization GetByName(string name)
        {
            string sql = "SELECT * FROM Organization WHERE name = @Name";
            
            using (var reader = ExecuteReader(sql, new MySqlParameter("@Name", name)))
            {
                if (reader.Read())
                {
                    return MapOrganizationFromReader(reader);
                }
            }
            return null;
        }

        public IEnumerable<Organization> GetAll()
        {
            var organizations = new List<Organization>();
            
            string sql = "SELECT * FROM Organization ORDER BY name";
            
            using (var reader = ExecuteReader(sql))
            {
                while (reader.Read())
                {
                    organizations.Add(MapOrganizationFromReader(reader));
                }
            }
            
            return organizations;
        }

        public bool Exists(int organizationId)
        {
            string sql = "SELECT COUNT(*) FROM Organization WHERE organization_id = @OrganizationId";
            var count = Convert.ToInt32(ExecuteScalar(sql, new MySqlParameter("@OrganizationId", organizationId)));
            return count > 0;
        }

        public bool OrganizationExists(string name)
        {
            string sql = "SELECT COUNT(*) FROM Organization WHERE name = @Name";
            var count = Convert.ToInt32(ExecuteScalar(sql, new MySqlParameter("@Name", name)));
            return count > 0;
        }

        private Organization MapOrganizationFromReader(MySqlDataReader reader)
        {
            var orgType = reader["organization_type"].ToString();
            var instituteType = reader["institute_type"] != DBNull.Value 
                ? reader["institute_type"].ToString() 
                : null;

            Organization organization;
            
            switch (orgType)
            {
                case "Company":
                    organization = new Company(
                        reader["name"].ToString(),
                        reader["address"].ToString(),
                        reader["phone_number"].ToString(),
                        reader["email"] != DBNull.Value ? reader["email"].ToString() : null,
                        reader["url"] != DBNull.Value ? reader["url"].ToString() : null,
                        null
                    );
                    break;
                case "ResearchGroup":
                    organization = new ResearchGroup(
                        reader["name"].ToString(),
                        reader["address"].ToString(),
                        reader["phone_number"].ToString(),
                        reader["email"] != DBNull.Value ? reader["email"].ToString() : null,
                        reader["url"] != DBNull.Value ? reader["url"].ToString() : null,
                        null
                    );
                    break;
                case "EducationalInstitute":
                InstituteType parsedInstituteType = InstituteType.WO;  // default
                    if (instituteType != null && Enum.TryParse<InstituteType>(instituteType, out var type))
                    {
                        parsedInstituteType = type;
                    }
                    organization = new EducationalInstitute(
                        reader["name"].ToString(),
                        reader["address"].ToString(),
                        reader["phone_number"].ToString(),
                        reader["email"] != DBNull.Value ? reader["email"].ToString() : null,
                        reader["url"] != DBNull.Value ? reader["url"].ToString() : null,
                        null,
                        parsedInstituteType
                    );
                    break;
                default:
                    throw new InvalidOperationException($"Unknown organization type: {orgType}");
            }

             organization.OrganizationId = Convert.ToInt32(reader["organization_id"]);
            
            return organization;
        }

        public bool ValidateOrganizationData(Organization organization)
        {
            // Check for required fields
            if (string.IsNullOrWhiteSpace(organization.Name) ||
                string.IsNullOrWhiteSpace(organization.Address) ||
                string.IsNullOrWhiteSpace(organization.Phone))
                return false;

            // Check if organization with same name exists (for new organizations)
            // if (organization.OrganizationId == 0 && OrganizationExists(organization.Name))
            //     return false;

            return true;
        }

        public List<Organization> SearchByName(string searchTerm)
        {
            var organizations = new List<Organization>();
            
            string sql = "SELECT * FROM Organization WHERE name LIKE @SearchTerm ORDER BY name";
            
            using (var reader = ExecuteReader(sql, new MySqlParameter("@SearchTerm", $"%{searchTerm}%")))
            {
                while (reader.Read())
                {
                    organizations.Add(MapOrganizationFromReader(reader));
                }
            }
            
            return organizations;
        }

        public List<Organization> GetByType(string organizationType)
        {
            var organizations = new List<Organization>();
            
            string sql = "SELECT * FROM Organization WHERE organization_type = @OrgType ORDER BY name";
            
            using (var reader = ExecuteReader(sql, new MySqlParameter("@OrgType", organizationType)))
            {
                while (reader.Read())
                {
                    organizations.Add(MapOrganizationFromReader(reader));
                }
            }
            
            return organizations;
        }

        public bool AddInternshipToOrganization(int organizationId, Internship internship)
        {
            throw new NotImplementedException();
        }

        public List<Internship> GetInternshipsByOrganizationId(int organizationId)
        {
            throw new NotImplementedException();
        }

        public List<ContactPerson> GetContactPersonsByOrganizationId(int organizationId)
        {
            throw new NotImplementedException();
        }

        // Note: GetInternshipsByOrganizationId and AddInternshipToOrganization 
        // are better handled in InternshipRepository
    }
}