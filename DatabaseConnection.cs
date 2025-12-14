// // DatabaseConnection.cs
// using MySql.Data.MySqlClient;
// using System;

// public class DatabaseConnection : IDisposable
// {
//     protected MySqlConnection connection;
//     private readonly string connectionString;

//     public DatabaseConnection(string connectionString)
//     {
//         this.connectionString = connectionString;
//     }

//     protected void OpenConnection()
//     {
//         try
//         {
//             if (connection == null)
//             {
//                 connection = new MySqlConnection(connectionString);
//             }
            
//             if (connection.State != System.Data.ConnectionState.Open)
//             {
//                 connection.Open();
//             }
//         }
//         catch (MySqlException ex)
//         {
//             Console.WriteLine($"Database Connection Error: {ex.Message}");
//             throw;
//         }
//     }

//     protected void CloseConnection()
//     {
//         if (connection != null && connection.State == System.Data.ConnectionState.Open)
//         {
//             connection.Close();
//         }
//     }

//     public void Dispose()
//     {
//         CloseConnection();
//         connection?.Dispose();
//     }
// }


// // // IRepository.cs
// // public interface IRepository<T> where T : class
// // {
// //     bool Add(T entity);
// //     bool Update(T entity);
// //     bool Delete(int id);
// //     T GetById(int id);
// //     IEnumerable<T> GetAll();
// // }

// // // PersonRepository.cs (Abstract)
// // public abstract class PersonRepository : DatabaseConnection
// // {
// //     protected PersonRepository(string connectionString) : base(connectionString) { }
    
// //     public abstract bool AddPerson(Person person);
// //     public abstract Person GetPersonById(int id);
// //     public abstract bool UpdatePerson(Person person);
// //     public abstract bool DeletePerson(int id);
// // }

// // // OrganizationRepository.cs (Abstract)
// // public abstract class OrganizationRepository : DatabaseConnection
// // {
// //     protected OrganizationRepository(string connectionString) : base(connectionString) { }
    
// //     public abstract bool AddOrganization(Organization organization);
// //     public abstract Organization GetOrganizationById(int id);
// //     public abstract IEnumerable<Organization> GetAllOrganizations();
// //     public abstract bool ValidateOrganizationData(Organization organization);
// // }


// // // MySQLPersonRepository.cs
// // public class MySQLPersonRepository : PersonRepository
// // {
// //     public MySQLPersonRepository(string connectionString) : base(connectionString) { }
    
// //     public override bool AddPerson(Person person)
// //     {
// //         try
// //         {
// //             OpenConnection();
            
// //             string query = @"INSERT INTO Persons (FirstName, LastName, PersonType) 
// //                             VALUES (@FirstName, @LastName, @PersonType)";
            
// //             using (var command = new MySqlCommand(query, connection))
// //             {
// //                 command.Parameters.AddWithValue("@FirstName", person.FirstName);
// //                 command.Parameters.AddWithValue("@LastName", person.LastName);
// //                 command.Parameters.AddWithValue("@PersonType", GetPersonType(person));
                
// //                 int rowsAffected = command.ExecuteNonQuery();
// //                 return rowsAffected > 0;
// //             }
// //         }
// //         catch (Exception ex)
// //         {
// //             Console.WriteLine($"Error adding person: {ex.Message}");
// //             return false;
// //         }
// //     }
    
// //     private string GetPersonType(Person person)
// //     {
// //         if (person is Student) return "Student";
// //         if (person is ContactPerson) return "ContactPerson";
// //         return "Person";
// //     }
    
// //     public override Person GetPersonById(int id)
// //     {
// //         // Implementation for retrieving a person by ID
// //     }
    
// //     // Implement other abstract methods...
// // }


// // // MySQLOrganizationRepository.cs
// // public class MySQLOrganizationRepository : OrganizationRepository
// // {
// //     public MySQLOrganizationRepository(string connectionString) : base(connectionString) { }
    
// //     public override bool AddOrganization(Organization organization)
// //     {
// //         try
// //         {
// //             OpenConnection();
            
// //             string query = @"INSERT INTO Organizations (Name, Address, Phone, Email, Url, OrganizationType) 
// //                             VALUES (@Name, @Address, @Phone, @Email, @Url, @OrgType)";
            
// //             using (var command = new MySqlCommand(query, connection))
// //             {
// //                 command.Parameters.AddWithValue("@Name", organization.Name);
// //                 command.Parameters.AddWithValue("@Address", organization.Address);
// //                 command.Parameters.AddWithValue("@Phone", organization.Phone);
// //                 command.Parameters.AddWithValue("@Email", organization.Email);
// //                 command.Parameters.AddWithValue("@Url", organization.Url);
// //                 command.Parameters.AddWithValue("@OrgType", GetOrganizationType(organization));
                
// //                 int rowsAffected = command.ExecuteNonQuery();
// //                 return rowsAffected > 0;
// //             }
// //         }
// //         catch (Exception ex)
// //         {
// //             Console.WriteLine($"Error adding organization: {ex.Message}");
// //             return false;
// //         }
// //     }
    
// //     private string GetOrganizationType(Organization org)
// //     {
// //         if (org is Company) return "Company";
// //         if (org is ResearchGroup) return "ResearchGroup";
// //         if (org is EducationalInstitute) return "EducationalInstitute";
// //         return "Organization";
// //     }
    
// //     public override IEnumerable<Organization> GetAllOrganizations()
// //     {
// //         var organizations = new List<Organization>();
        
// //         try
// //         {
// //             OpenConnection();
            
// //             string query = "SELECT * FROM Organizations";
// //             using (var command = new MySqlCommand(query, connection))
// //             using (var reader = command.ExecuteReader())
// //             {
// //                 while (reader.Read())
// //                 {
// //                     Organization org = CreateOrganizationFromReader(reader);
// //                     organizations.Add(org);
// //                 }
// //             }
// //         }
// //         catch (Exception ex)
// //         {
// //             Console.WriteLine($"Error retrieving organizations: {ex.Message}");
// //         }
        
// //         return organizations;
// //     }
    
// //     private Organization CreateOrganizationFromReader(MySqlDataReader reader)
// //     {
// //         // Logic to create the appropriate Organization subtype based on OrganizationType
// //     }
    
// //     // Implement other abstract methods...
// // }

