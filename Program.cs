using System;
using SIS.Models;
using SIS.Models.Assignment;
using SIS.Models.Internship;
using SIS.Models.Organization;
using SIS.Models.Period;
using SIS.Services;
using SIS.Menus;
using SIS.Interfaces;

namespace SIS
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Student Internship System ===");
            Console.WriteLine("Select User Type:");
            Console.WriteLine("1. Student");
            Console.WriteLine("2. Coordinator");
            Console.Write("Choice: ");
            
            var service = new InternshipService();
            InitializeSampleData(service);
            
            IMenu menu = null;
            
            switch (Console.ReadLine())
            {
                case "1":
                    Console.Write("Enter Student Number: ");
                    if (int.TryParse(Console.ReadLine(), out int studentNumber))
                    {
                        var student = service.GetStudentByNumber(studentNumber);
                        if (student == null)
                        {
                            Console.WriteLine("Student not found. Creating new student...");
                            Console.Write("First Name: ");
                            var firstName = Console.ReadLine();
                            Console.Write("Last Name: ");
                            var lastName = Console.ReadLine();
                            student = new Student(firstName, lastName, studentNumber);
                            service.RegisterStudent(student);
                        }
                        menu = new StudentMenu(service, student);
                    }
                    break;
                    
                case "2":
                    //var coordinator = new Coordinator("John", "Doe", 1001);
                    menu = new CoordinatorMenu(service);
                    break;
                    
                default:
                    Console.WriteLine("Invalid choice.");
                    return;
            }
            
            if (menu != null)
            {
                while (true)
                {
                    menu.ShowMenu();
                    var choice = Console.ReadLine();
                    menu.HandleChoice(choice);
                }
            }
        }
        
        static void InitializeSampleData(InternshipService service)
        {
            Console.WriteLine("\n=== INITIALIZING SAMPLE DATA ===\n");
    
    // Sample Student
    var student1 = new Student("Alice", "Smith", 12345);
    service.RegisterStudent(student1);
    Console.WriteLine($"✓ Created Student: {student1.FullName} (Student Number: {student1.StudentNumber})");
    
    var student2 = new Student("Bob", "Wilson", 67890);
    service.RegisterStudent(student2);
    Console.WriteLine($"✓ Created Student: {student2.FullName} (Student Number: {student2.StudentNumber})");
    
    // Sample Organizations
    var contactPerson1 = new ContactPerson("Bob", "Johnson", "HR Manager", 
        "Human Resources", "+1234567890", "bob.johnson@tech.com");
    var company = new Company("Tech Solutions Inc.", "123 Tech Street", 
        "+1234567890", "info@techsolutions.com", "https://techsolutions.com", contactPerson1);
    service.AddOrganization(company);
    Console.WriteLine($"✓ Created Organization: {company.Name} (ID: {company.OrganizationId})");
    
    var contactPerson2 = new ContactPerson("Sarah", "Miller", "Research Lead", 
        "R&D", "+0987654321", "sarah.miller@research.edu");
    var researchGroup = new ResearchGroup("AI Research Lab", "456 Science Ave", 
        "+0987654321", "contact@airesearch.edu", "https://airesearch.edu", contactPerson2);
    service.AddOrganization(researchGroup);
    Console.WriteLine($"✓ Created Organization: {researchGroup.Name} (ID: {researchGroup.OrganizationId})");
    
    // Sample Internships
    var period2025I = new Period(2025, Semester.I);
    var period2025II = new Period(2025, Semester.II);
    
    var internship1 = new IntermediateInternship(company, "Amsterdam", 
        DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), "AI Development Project",
        "Develop AI algorithms for business optimization", 
        "Detailed description of AI development project...", period2025I);
    internship1.AddContactPerson(contactPerson1);
    service.AddInternship(internship1);
    Console.WriteLine($"✓ Created Internship: '{internship1.ProjectTitle}' (ID: {internship1.InternshipId}) in {period2025I}");
    
    var internship2 = new GraduationInternship(researchGroup, "Utrecht", 
        DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - 86400000, "Machine Learning Research",
        "Research on neural network optimization", 
        "Detailed research project on ML optimization...", period2025II);
    internship2.AddContactPerson(contactPerson2);
    service.AddInternship(internship2);
    Console.WriteLine($"✓ Created Internship: '{internship2.ProjectTitle}' (ID: {internship2.InternshipId}) in {period2025II}");
    
    Console.WriteLine("\n=== SAMPLE DATA SUMMARY ===");
    Console.WriteLine($"Students: 2 (IDs: 12345, 67890)");
    Console.WriteLine($"Organizations: 2 (IDs: {company.OrganizationId}, {researchGroup.OrganizationId})");
    Console.WriteLine($"Internships: 2 (IDs: {internship1.InternshipId}, {internship2.InternshipId})");
    Console.WriteLine("============================\n");
        }
    }
}