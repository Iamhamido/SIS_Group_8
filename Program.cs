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
            // Sample Student
            var student1 = new Student("Alice", "Smith", 12345);
            service.RegisterStudent(student1);
            
            // Sample Organization
            var contactPerson1 = new ContactPerson("Bob", "Johnson", "HR Manager", 
                "Human Resources", "+1234567890", "bob.johnson@company.com");
            
            var company = new Company("Tech Solutions Inc.", "123 Tech Street", 
                "+1234567890", "info@techsolutions.com", "https://techsolutions.com", contactPerson1);
            service.AddOrganization(company);
            
            // Sample Internship
            var period = new Period(2025, Semester.I);
            var internship = new IntermediateInternship(company, "Amsterdam", 
                DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), "AI Development Project",
                "Develop AI algorithms for business optimization", 
                "Detailed description of AI development project...", period);
            
            internship.AddContactPerson(contactPerson1);
            service.AddInternship(internship);
        }
    }
}