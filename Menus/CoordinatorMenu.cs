using System;
using System.Collections.Generic;
using System.Linq;
using SIS.Interfaces;
using SIS.Models;
using SIS.Models.Assignment;
using SIS.Models.Internship;
using SIS.Models.Organization;
using SIS.Models.Period;
using SIS.Services;

namespace SIS.Menus
{
    public class CoordinatorMenu : IMenu
    {
        private InternshipService _service;
        //private Coordinator _coordinator;

        public CoordinatorMenu(InternshipService service)
        {
            _service = service;
            //_coordinator = coordinator;
        }

        public void ShowMenu()
        {
            Console.Clear();
            Console.WriteLine("=== COORDINATOR INTERNSHIP SYSTEM ===");
            Console.WriteLine($"Welcome, Coordinator");
            Console.WriteLine("=====================================");
            Console.WriteLine("1. Add Organization");
            Console.WriteLine("2. Add Internship");
            Console.WriteLine("3. View All Organizations");
            Console.WriteLine("4. View Internships by Organization");
            Console.WriteLine("5. View Internships by Period and Type");
            Console.WriteLine("6. Add Assignment");
            Console.WriteLine("7. Remove Assignment");
            Console.WriteLine("8. Grade Assignment");
            Console.WriteLine("9. View Contact Persons by Internship");
            Console.WriteLine("10. Withdraw Internship");
            Console.WriteLine("11. Exit");
            Console.WriteLine("=====================================");
            Console.Write("Enter your choice: ");
        }

        public void HandleChoice(string choice)
        {
            switch (choice)
            {
                case "1":
                    AddOrganization();
                    break;
                case "2":
                    AddInternship();
                    break;
                case "3":
                    ViewAllOrganizations();
                    break;
                case "4":
                    ViewInternshipsByOrganizationId();
                    break;
                case "5":
                    GetInternshipsByPeriodAndType();
                    break;
                case "6":
                    AddAssignment();
                    break;
                case "7":
                    RemoveAssignment();
                    break;
                case "8":
                    GradeAssignment();
                    break;
                case "9":
                    ViewContactPersonsByInternshipId();
                    break;
                case "10":
                    WithdrawInternship();
                    break;
                case "11":
                    Console.WriteLine("Goodbye!");
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }

        private void AddOrganization()
        {
            Console.WriteLine("\n=== ADD NEW ORGANIZATION ===");
            
            Console.Write("Organization Name: ");
            var name = Console.ReadLine();
            
            Console.Write("Address: ");
            var address = Console.ReadLine();
            
            Console.Write("Phone: ");
            var phone = Console.ReadLine();
            
            Console.Write("Email: ");
            var email = Console.ReadLine();
            
            Console.Write("URL: ");
            var url = Console.ReadLine();
            
            Console.WriteLine("\n=== CONTACT PERSON DETAILS ===");
            Console.Write("First Name: ");
            var cpFirstName = Console.ReadLine();
            
            Console.Write("Last Name: ");
            var cpLastName = Console.ReadLine();
            
            Console.Write("Function Title: ");
            var cpTitle = Console.ReadLine();
            
            Console.Write("Department: ");
            var cpDept = Console.ReadLine();
            
            Console.Write("Phone: ");
            var cpPhone = Console.ReadLine();
            
            Console.Write("Email: ");
            var cpEmail = Console.ReadLine();
            
            Console.WriteLine("\n=== ORGANIZATION TYPE ===");
            Console.WriteLine("1. Company");
            Console.WriteLine("2. Research Group");
            Console.WriteLine("3. Educational Institute");
            Console.Write("Choice: ");
            
            var contactPerson = new ContactPerson(cpFirstName, cpLastName, cpTitle, cpDept, cpPhone, cpEmail);
            Organization organization = null;
            
            switch (Console.ReadLine())
            {
                case "1":
                    organization = new Company(name, address, phone, email, url, contactPerson);
                    break;
                case "2":
                    organization = new ResearchGroup(name, address, phone, email, url, contactPerson);
                    break;
                case "3":
                    Console.WriteLine("Institute Type (WO, HBO, INTERNATIONAL): ");
                    if (!Enum.TryParse<InstituteType>(Console.ReadLine(), out var instituteType))
                    {
                        Console.WriteLine("Invalid institute type.");
                        return;
                    }
                    organization = new EducationalInstitute(name, address, phone, email, url, contactPerson, instituteType);
                    break;
                default:
                    Console.WriteLine("Invalid choice.");
                    return;
            }
            
            if (_service.AddOrganization(organization))
            {
                Console.WriteLine($"Organization added successfully! ID: {organization.OrganizationId}");
            }
            else
            {
                Console.WriteLine("Failed to add organization. Check if data is valid and not duplicated.");
            }
            
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private void AddInternship()
        {
            Console.WriteLine("\n=== ADD NEW INTERNSHIP ===");
            
            Console.Write("Organization ID: ");
            if (!int.TryParse(Console.ReadLine(), out int orgId))
            {
                Console.WriteLine("Invalid ID.");
                return;
            }
            
            var organizations = _service.GetAllOrganizations();
            var organization = organizations.FirstOrDefault(o => o.OrganizationId == orgId);
            if (organization == null)
            {
                Console.WriteLine("Organization not found.");
                return;
            }
            
            Console.Write("City: ");
            var city = Console.ReadLine();
            
            var dateOfSubmission = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            
            Console.Write("Project Title: ");
            var projectTitle = Console.ReadLine();
            
            Console.Write("Short Description: ");
            var shortDesc = Console.ReadLine();
            
            Console.Write("Long Description: ");
            var longDesc = Console.ReadLine();
            
            Console.Write("Period (e.g., 2025-I): ");
            var periodInput = Console.ReadLine();
            
            if (!TryParsePeriod(periodInput, out var period))
            {
                Console.WriteLine("Invalid period.");
                return;
            }
            
            Console.WriteLine("Internship Type:");
            Console.WriteLine("1. Intermediate Internship (ENGINEERING)");
            Console.WriteLine("2. Minor Internship");
            Console.WriteLine("3. Graduation Internship (ENGINEERING)");
            Console.Write("Choice: ");
            
            Internship internship = null;
            
            switch (Console.ReadLine())
            {
                case "1":
                    internship = new IntermediateInternship(organization, city, dateOfSubmission, 
                                                          projectTitle, shortDesc, longDesc, period);
                    break;
                case "2":
                    if (organization is EducationalInstitute eduInstitute)
                    {
                        Console.Write("Minor ID: ");
                        if (!int.TryParse(Console.ReadLine(), out int minorId))
                        {
                            Console.WriteLine("Invalid Minor ID.");
                            return;
                        }
                        internship = new MinorInternship(eduInstitute, city, dateOfSubmission,
                                                       projectTitle, shortDesc, longDesc, period, minorId);
                    }
                    else
                    {
                        Console.WriteLine("Minor internships can only be with Educational Institutes.");
                        return;
                    }
                    break;
                case "3":
                    internship = new GraduationInternship(organization, city, dateOfSubmission,
                                                        projectTitle, shortDesc, longDesc, period);
                    break;
                default:
                    Console.WriteLine("Invalid choice.");
                    return;
            }
            
            // Add contact persons
            Console.WriteLine("\nAdd Contact Persons (enter 'done' when finished):");
            while (true)
            {
                Console.Write("First Name (or 'done'): ");
                var firstName = Console.ReadLine();
                if (firstName.ToLower() == "done") break;
                
                Console.Write("Last Name: ");
                var lastName = Console.ReadLine();
                
                Console.Write("Function Title: ");
                var title = Console.ReadLine();
                
                Console.Write("Department: ");
                var dept = Console.ReadLine();
                
                Console.Write("Phone: ");
                var phone = Console.ReadLine();
                
                Console.Write("Email: ");
                var email = Console.ReadLine();
                
                var contactPerson = new ContactPerson(firstName, lastName, title, dept, phone, email);
                internship.AddContactPerson(contactPerson);
            }
            
            if (_service.AddInternship(internship))
            {
                Console.WriteLine($"Internship added successfully! ID: {internship.InternshipId}");
            }
            else
            {
                Console.WriteLine("Failed to add internship.");
            }
            
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        public void ViewAllOrganizations()
        {
            var organizations = _service.GetAllOrganizations();
            
            Console.WriteLine("\n=== ALL ORGANIZATIONS ===");
            Console.WriteLine($"Total: {organizations.Count}");
            Console.WriteLine();
            
            foreach (var org in organizations)
            {
                Console.WriteLine($"ID: {org.OrganizationId}");
                Console.WriteLine($"Name: {org.Name}");
                Console.WriteLine($"Type: {org.GetType().Name}");
                Console.WriteLine($"Address: {org.Address}");
                Console.WriteLine($"Email: {org.Email}");
                Console.WriteLine($"Phone: {org.Phone}");
                Console.WriteLine($"Contact: {org.ContactPerson.FullName} ({org.ContactPerson.Email})");
                Console.WriteLine($"Internships: {org.Internships.Count}");
                Console.WriteLine();
            }
            
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        public void ViewInternshipsByOrganizationId()
        {
            Console.Write("Enter Organization ID: ");
            if (!int.TryParse(Console.ReadLine(), out int organizationId))
            {
                Console.WriteLine("Invalid ID.");
                return;
            }

            var internships = _service.GetInternshipsByOrganizationId(organizationId);
            
            Console.WriteLine($"\n=== INTERNSHIPS FOR ORGANIZATION ID: {organizationId} ===");
            Console.WriteLine($"Total: {internships.Count}");
            Console.WriteLine();
            
            foreach (var internship in internships)
            {
                Console.WriteLine($"ID: {internship.InternshipId}");
                Console.WriteLine($"Project: {internship.ProjectTitle}");
                Console.WriteLine($"City: {internship.City}");
                Console.WriteLine($"Period: {internship.Period}");
                Console.WriteLine($"Type: {internship.AssignmentType}");
                Console.WriteLine($"Active: {internship.IsActive}");
                Console.WriteLine($"Students Enrolled: {internship.EnrolledStudents.Count}");
                Console.WriteLine();
            }
            
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        public void GetInternshipsByPeriodAndType()
        {
            Console.Write("Enter period (e.g., 2025-I): ");
            var periodInput = Console.ReadLine();
            
            if (!TryParsePeriod(periodInput, out var period))
            {
                Console.WriteLine("Invalid period.");
                return;
            }

            Console.WriteLine("Assignment Type (RESEARCH, MINOR, ENGINEERING): ");
            if (!Enum.TryParse<AssignmentType>(Console.ReadLine(), out var type))
            {
                Console.WriteLine("Invalid type.");
                return;
            }

            var internships = _service.GetInternshipsByPeriodAndType(period, type);
            
            Console.WriteLine($"\n=== INTERNSHIPS FOR {period}, TYPE: {type} ===");
            Console.WriteLine($"Total: {internships.Count}");
            Console.WriteLine();
            
            foreach (var internship in internships)
            {
                Console.WriteLine($"ID: {internship.InternshipId}");
                Console.WriteLine($"Organization: {internship.Organization.Name}");
                Console.WriteLine($"Project: {internship.ProjectTitle}");
                Console.WriteLine($"City: {internship.City}");
                Console.WriteLine($"Students: {internship.EnrolledStudents.Count}");
                Console.WriteLine();
            }
            
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private bool AddAssignment()
        {
            Console.WriteLine("\n=== ADD ASSIGNMENT ===");
            
            Console.Write("Student Number: ");
            if (!int.TryParse(Console.ReadLine(), out int studentNumber))
            {
                Console.WriteLine("Invalid student number.");
                return false;
            }
            
            var student = _service.GetStudentByNumber(studentNumber);
            if (student == null)
            {
                Console.WriteLine("Student not found.");
                return false;
            }
            
            Console.Write("Internship ID: ");
            if (!int.TryParse(Console.ReadLine(), out int internshipId))
            {
                Console.WriteLine("Invalid internship ID.");
                return false;
            }
            
            var internship = _service.GetInternshipById(internshipId);
            if (internship == null || !internship.IsActive)
            {
                Console.WriteLine("Internship not found or inactive.");
                return false;
            }
            
            Console.WriteLine("Assignment Type (RESEARCH, MINOR, ENGINEERING): ");
            if (!Enum.TryParse<AssignmentType>(Console.ReadLine(), out var assignmentType))
            {
                Console.WriteLine("Invalid assignment type.");
                return false;
            }
            
            var assignment = new Assignment(student, internship, assignmentType);
            
            if (_service.AddAssignment(assignment))
            {
                Console.WriteLine("Assignment added successfully!");
                return true;
            }
            else
            {
                Console.WriteLine("Failed to add assignment. Student may already have an assignment in this period.");
                return false;
            }
        }

        private bool RemoveAssignment()
        {
            Console.WriteLine("\n=== REMOVE ASSIGNMENT ===");
            
            Console.Write("Student Number: ");
            if (!int.TryParse(Console.ReadLine(), out int studentNumber))
            {
                Console.WriteLine("Invalid student number.");
                return false;
            }
            
            var student = _service.GetStudentByNumber(studentNumber);
            if (student == null)
            {
                Console.WriteLine("Student not found.");
                return false;
            }
            
            var assignments = _service.GetAssignmentsForStudent(studentNumber);
            if (assignments.Count == 0)
            {
                Console.WriteLine("No assignments found for this student.");
                return false;
            }
            
            Console.WriteLine("Select assignment to remove:");
            for (int i = 0; i < assignments.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {assignments[i]}");
            }
            
            if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 1 || choice > assignments.Count)
            {
                Console.WriteLine("Invalid choice.");
                return false;
            }
            
            var assignment = assignments[choice - 1];
            
            if (_service.RemoveAssignment(assignment))
            {
                Console.WriteLine("Assignment removed successfully!");
                return true;
            }
            else
            {
                Console.WriteLine("Failed to remove assignment.");
                return false;
            }
        }

        private bool GradeAssignment()
        {
            Console.WriteLine("\n=== GRADE ASSIGNMENT ===");
            
            Console.Write("Student Number: ");
            if (!int.TryParse(Console.ReadLine(), out int studentNumber))
            {
                Console.WriteLine("Invalid student number.");
                return false;
            }
            
            var student = _service.GetStudentByNumber(studentNumber);
            if (student == null)
            {
                Console.WriteLine("Student not found.");
                return false;
            }
            
            Console.Write("Grade (1.0-10.0): ");
            if (!double.TryParse(Console.ReadLine(), out double grade) || grade < 1.0 || grade > 10.0)
            {
                Console.WriteLine("Invalid grade. Must be between 1.0 and 10.0.");
                return false;
            }
            
            if (_service.GradeAssignment(student, grade))
            {
                Console.WriteLine("Grade assigned successfully!");
                return true;
            }
            else
            {
                Console.WriteLine("Failed to assign grade.");
                return false;
            }
        }

        private void ViewContactPersonsByInternshipId()
        {
            Console.Write("Enter Internship ID: ");
            if (!int.TryParse(Console.ReadLine(), out int internshipId))
            {
                Console.WriteLine("Invalid ID.");
                return;
            }

            var contactPersons = _service.GetContactPersonsByInternshipId(internshipId);
            
            Console.WriteLine($"\n=== CONTACT PERSONS FOR INTERNSHIP ID: {internshipId} ===");
            Console.WriteLine($"Total: {contactPersons.Count}");
            Console.WriteLine();
            
            foreach (var cp in contactPersons)
            {
                Console.WriteLine($"Name: {cp.FullName}");
                Console.WriteLine($"Title: {cp.FunctionTitle}");
                Console.WriteLine($"Department: {cp.Department}");
                Console.WriteLine($"Email: {cp.Email}");
                Console.WriteLine($"Phone: {cp.Phone}");
                Console.WriteLine();
            }
            
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private void WithdrawInternship()
        {
            Console.Write("Enter Internship ID to withdraw: ");
            if (!int.TryParse(Console.ReadLine(), out int internshipId))
            {
                Console.WriteLine("Invalid ID.");
                return;
            }

            Console.Write("Are you sure you want to withdraw this internship? (yes/no): ");
            if (Console.ReadLine().ToLower() != "yes")
            {
                Console.WriteLine("Withdrawal cancelled.");
                return;
            }
            
            if (_service.WithdrawInternship(internshipId))
            {
                Console.WriteLine("Internship withdrawn successfully!");
            }
            else
            {
                Console.WriteLine("Failed to withdraw internship.");
            }
            
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private bool TryParsePeriod(string input, out Period period)
        {
            period = null;
            
            if (string.IsNullOrWhiteSpace(input))
                return false;
                
            var parts = input.Split('-');
            if (parts.Length != 2)
                return false;
                
            if (!int.TryParse(parts[0], out int year))
                return false;
                
            if (!Enum.TryParse<Semester>(parts[1], out var semester))
                return false;
                
            period = new Period(year, semester);
            return true;
        }
    }
}