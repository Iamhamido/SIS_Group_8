// using System.Collections.Generic;
// using System.Linq;
// using MySql.Data.MySqlClient;
// using SIS.Models;
// using SIS.Repositories;

// namespace SIS.Data.Repositories
// {
//     public class MySQLContactPersonRepository : DatabaseContext, IContactPersonRepository
//     {
//         private readonly IOrganizationRepository _organizationRepository;
        
//         public MySQLContactPersonRepository(string connectionString, IOrganizationRepository orgRepo) 
//             : base(connectionString)
//         {
//             _organizationRepository = orgRepo;
//         }

//         public bool Add(ContactPerson contactPerson)
//         {
//             // For ContactPerson, we need organizationId
//             if (contactPerson.Organization == null || contactPerson.Organization.OrganizationId <= 0)
//                 return false;
                
//             return AddContactPerson(contactPerson, contactPerson.Organization.OrganizationId);
//         }

//         public bool AddContactPerson(ContactPerson contactPerson, int organizationId)
//         {
//             try
//             {
//                 // Insert into Person table
//                 string personSql = @"INSERT INTO Person (First_name, Last_name, PersonType) 
//                                     VALUES (@FirstName, @LastName, 'ContactPerson')";
                
//                 var personParams = new[]
//                 {
//                     new MySqlParameter("@FirstName", contactPerson.FirstName),
//                     new MySqlParameter("@LastName", contactPerson.LastName)
//                 };

//                 ExecuteNonQuery(personSql, personParams);

//                 // Get the auto-generated Person_id
//                 int personId = Convert.ToInt32(ExecuteScalar("SELECT LAST_INSERT_ID()"));

//                 // Insert into Contact_person table
//                 string contactSql = @"INSERT INTO Contact_person 
//                                     (person_id, organization_id, function_title, email, phone_number, department_name) 
//                                     VALUES (@PersonId, @OrgId, @FunctionTitle, @Email, @Phone, @Department)";
                
//                 var contactParams = new[]
//                 {
//                     new MySqlParameter("@PersonId", personId),
//                     new MySqlParameter("@OrgId", organizationId),
//                     new MySqlParameter("@FunctionTitle", contactPerson.FunctionTitle),
//                     new MySqlParameter("@Email", contactPerson.Email),
//                     new MySqlParameter("@Phone", contactPerson.Phone),
//                     new MySqlParameter("@Department", contactPerson.Department)
//                 };

//                 ExecuteNonQuery(contactSql, contactParams);

//                 // Get the auto-generated contact_person_id
//                 int contactPersonId = Convert.ToInt32(ExecuteScalar("SELECT LAST_INSERT_ID()"));
                
//                 contactPerson.ContactPersonId = contactPersonId;
//                 contactPerson.PersonId = personId;
                
//                 return true;
//             }
//             catch (MySqlException ex)
//             {
//                 Console.WriteLine($"Error adding contact person: {ex.Message}");
//                 return false;
//             }
//         }

//         public bool Update(ContactPerson contactPerson)
//         {
//             // Update Person information
//             string personSql = @"UPDATE Person 
//                                 SET First_name = @FirstName, Last_name = @LastName
//                                 WHERE Person_id = (SELECT person_id FROM Contact_person WHERE contact_person_id = @ContactPersonId)";
            
//             // Update Contact_person information
//             string contactSql = @"UPDATE Contact_person 
//                                 SET function_title = @FunctionTitle,
//                                     email = @Email,
//                                     phone_number = @Phone,
//                                     department_name = @Department,
//                                     organization_id = @OrgId
//                                 WHERE contact_person_id = @ContactPersonId";

//             try
//             {
//                 // Update Person
//                 var personParams = new[]
//                 {
//                     new MySqlParameter("@FirstName", contactPerson.FirstName),
//                     new MySqlParameter("@LastName", contactPerson.LastName),
//                     new MySqlParameter("@ContactPersonId", contactPerson.ContactPersonId)
//                 };
//                 ExecuteNonQuery(personSql, personParams);

//                 // Update Contact_person
//                 var contactParams = new[]
//                 {
//                     new MySqlParameter("@FunctionTitle", contactPerson.FunctionTitle),
//                     new MySqlParameter("@Email", contactPerson.Email),
//                     new MySqlParameter("@Phone", contactPerson.Phone),
//                     new MySqlParameter("@Department", contactPerson.Department),
//                     new MySqlParameter("@OrgId", contactPerson.Organization?.OrganizationId ?? 0),
//                     new MySqlParameter("@ContactPersonId", contactPerson.ContactPersonId)
//                 };
//                 ExecuteNonQuery(contactSql, contactParams);

//                 return true;
//             }
//             catch (MySqlException ex)
//             {
//                 Console.WriteLine($"Error updating contact person: {ex.Message}");
//                 return false;
//             }
//         }

//         public bool Delete(int contactPersonId)
//         {
//             try
//             {
//                 // Get Person_id for this contact person
//                 string getPersonIdSql = "SELECT person_id FROM Contact_person WHERE contact_person_id = @ContactPersonId";
//                 var personIdObj = ExecuteScalar(getPersonIdSql, new MySqlParameter("@ContactPersonId", contactPersonId));
                
//                 if (personIdObj == null) return false;
                
//                 int personId = Convert.ToInt32(personIdObj);

//                 // Remove from contact_person_internship table first
//                 string removeCpiSql = "DELETE FROM contact_person_internship WHERE contact_person_id = @ContactPersonId";
//                 ExecuteNonQuery(removeCpiSql, new MySqlParameter("@ContactPersonId", contactPersonId));

//                 // Delete from Contact_person table
//                 string deleteContactSql = "DELETE FROM Contact_person WHERE contact_person_id = @ContactPersonId";
//                 ExecuteNonQuery(deleteContactSql, new MySqlParameter("@ContactPersonId", contactPersonId));

//                 // Delete from Person table
//                 string deletePersonSql = "DELETE FROM Person WHERE Person_id = @PersonId";
//                 ExecuteNonQuery(deletePersonSql, new MySqlParameter("@PersonId", personId));

//                 return true;
//             }
//             catch (MySqlException ex)
//             {
//                 Console.WriteLine($"Error deleting contact person: {ex.Message}");
//                 return false;
//             }
//         }

//         public ContactPerson GetById(int contactPersonId)
//         {
//             string sql = @"SELECT cp.contact_person_id, cp.function_title, cp.email, 
//                                   cp.phone_number, cp.department_name, cp.organization_id,
//                                   p.Person_id, p.First_name, p.Last_name
//                           FROM Contact_person cp
//                           INNER JOIN Person p ON cp.person_id = p.Person_id
//                           WHERE cp.contact_person_id = @ContactPersonId";
            
//             using (var reader = ExecuteReader(sql, new MySqlParameter("@ContactPersonId", contactPersonId)))
//             {
//                 if (reader.Read())
//                 {
//                     var contactPerson = new ContactPerson
//                     {
//                         ContactPersonId = Convert.ToInt32(reader["contact_person_id"]),
//                         PersonId = Convert.ToInt32(reader["Person_id"]),
//                         FirstName = reader["First_name"].ToString(),
//                         LastName = reader["Last_name"].ToString(),
//                         FunctionTitle = reader["function_title"].ToString(),
//                         Email = reader["email"].ToString(),
//                         Phone = reader["phone_number"].ToString(),
//                         Department = reader["department_name"].ToString()
//                     };

//                     // Get organization
//                     var orgId = Convert.ToInt32(reader["organization_id"]);
//                     var org = _organizationRepository.GetById(orgId);
//                     contactPerson.Organization = org;

//                     return contactPerson;
//                 }
//             }
//             return null;
//         }

//         public ContactPerson GetByEmail(string email)
//         {
//             string sql = @"SELECT cp.contact_person_id
//                           FROM Contact_person cp
//                           WHERE cp.email = @Email";
            
//             var contactPersonIdObj = ExecuteScalar(sql, new MySqlParameter("@Email", email));
            
//             if (contactPersonIdObj == null) return null;
            
//             int contactPersonId = Convert.ToInt32(contactPersonIdObj);
//             return GetById(contactPersonId);
//         }

//         public IEnumerable<ContactPerson> GetAll()
//         {
//             var contactPersons = new List<ContactPerson>();
            
//             string sql = @"SELECT cp.contact_person_id
//                           FROM Contact_person cp
//                           ORDER BY cp.contact_person_id";
            
//             using (var reader = ExecuteReader(sql))
//             {
//                 while (reader.Read())
//                 {
//                     int contactPersonId = Convert.ToInt32(reader["contact_person_id"]);
//                     var contactPerson = GetById(contactPersonId);
//                     if (contactPerson != null)
//                         contactPersons.Add(contactPerson);
//                 }
//             }
            
//             return contactPersons;
//         }

//         public bool Exists(int contactPersonId)
//         {
//             string sql = "SELECT COUNT(*) FROM Contact_person WHERE contact_person_id = @ContactPersonId";
//             var count = Convert.ToInt32(ExecuteScalar(sql, new MySqlParameter("@ContactPersonId", contactPersonId)));
//             return count > 0;
//         }

//         public List<ContactPerson> GetByName(string firstName, string lastName)
//         {
//             var contactPersons = new List<ContactPerson>();
            
//             string sql = @"SELECT cp.contact_person_id
//                           FROM Contact_person cp
//                           INNER JOIN Person p ON cp.person_id = p.Person_id
//                           WHERE p.First_name = @FirstName AND p.Last_name = @LastName";
            
//             var parameters = new[]
//             {
//                 new MySqlParameter("@FirstName", firstName),
//                 new MySqlParameter("@LastName", lastName)
//             };
            
//             using (var reader = ExecuteReader(sql, parameters))
//             {
//                 while (reader.Read())
//                 {
//                     int contactPersonId = Convert.ToInt32(reader["contact_person_id"]);
//                     var contactPerson = GetById(contactPersonId);
//                     if (contactPerson != null)
//                         contactPersons.Add(contactPerson);
//                 }
//             }
            
//             return contactPersons;
//         }

//         public List<ContactPerson> GetByOrganization(int organizationId)
//         {
//             var contactPersons = new List<ContactPerson>();
            
//             string sql = @"SELECT cp.contact_person_id
//                           FROM Contact_person cp
//                           WHERE cp.organization_id = @OrganizationId
//                           ORDER BY cp.contact_person_id";
            
//             using (var reader = ExecuteReader(sql, new MySqlParameter("@OrganizationId", organizationId)))
//             {
//                 while (reader.Read())
//                 {
//                     int contactPersonId = Convert.ToInt32(reader["contact_person_id"]);
//                     var contactPerson = GetById(contactPersonId);
//                     if (contactPerson != null)
//                         contactPersons.Add(contactPerson);
//                 }
//             }
            
//             return contactPersons;
//         }

//         public bool AddContactPersonToInternship(int contactPersonId, int internshipId)
//         {
//             try
//             {
//                 string sql = @"INSERT INTO contact_person_internship (contact_person_id, internship_id) 
//                               VALUES (@ContactPersonId, @InternshipId)";
                
//                 var parameters = new[]
//                 {
//                     new MySqlParameter("@ContactPersonId", contactPersonId),
//                     new MySqlParameter("@InternshipId", internshipId)
//                 };

//                 ExecuteNonQuery(sql, parameters);
//                 return true;
//             }
//             catch (MySqlException ex)
//             {
//                 // Handle duplicate entry
//                 if (ex.Number == 1062) // Duplicate entry
//                     return true; // Already exists, consider it successful
                    
//                 Console.WriteLine($"Error adding contact person to internship: {ex.Message}");
//                 return false;
//             }
//         }

//         public List<ContactPerson> GetContactPersonsByInternshipId(int internshipId)
//         {
//             var contactPersons = new List<ContactPerson>();
            
//             string sql = @"SELECT cp.contact_person_id
//                           FROM Contact_person cp
//                           INNER JOIN contact_person_internship cpi ON cp.contact_person_id = cpi.contact_person_id
//                           WHERE cpi.internship_id = @InternshipId";
            
//             using (var reader = ExecuteReader(sql, new MySqlParameter("@InternshipId", internshipId)))
//             {
//                 while (reader.Read())
//                 {
//                     int contactPersonId = Convert.ToInt32(reader["contact_person_id"]);
//                     var contactPerson = GetById(contactPersonId);
//                     if (contactPerson != null)
//                         contactPersons.Add(contactPerson);
//                 }
//             }
            
//             return contactPersons;
//         }
//     }
// }