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
    public class StudentMenu : IMenu
    {
        private InternshipService _service;
        private Student _student;
        private List<Assignment> _assignments;

        public StudentMenu(InternshipService service, Student student)
        {
            _service = service;
            _student = student;
            _assignments = new List<Assignment>();
        }

        public void ShowMenu()
        {
            Console.Clear();
            Console.WriteLine("=== STUDENT INTERNSHIP SYSTEM ===");
            Console.WriteLine($"Welcome, {_student.FullName} ({_student.StudentNumber})");
            Console.WriteLine("=================================");
            Console.WriteLine("1. View Available Internships");
            Console.WriteLine("2. View Internship Details");
            Console.WriteLine("3. Apply for Internship");
            Console.WriteLine("4. View My Assignments");
            Console.WriteLine("5. View All Organizations");
            Console.WriteLine("6. View Internships by Organization");
            Console.WriteLine("7. View Internships by Period and Type");
            Console.WriteLine("8. Exit");
            Console.WriteLine("=================================");
            Console.Write("Enter your choice: ");
        }

        public void HandleChoice(string choice)
        {
            switch (choice)
            {
                case "1":
                    ViewAvailableInternships();
                    break;
                case "2":
                    ViewInternshipDetails();
                    break;
                case "3":
                    ApplyForInternship();
                    break;
                case "4":
                    ViewMyAssignments();
                    break;
                case "5":
                    ViewAllOrganizations();
                    break;
                case "6":
                    ViewInternshipsByOrganizationId();
                    break;
                case "7":
                    GetInternshipsByPeriodAndType();
                    break;
                case "8":
                    Console.WriteLine("Goodbye!");
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }

        private void ViewAvailableInternships()
        {
            Console.Write("Enter period (e.g., 2025-I): ");
            var periodInput = Console.ReadLine();
            
            if (!TryParsePeriod(periodInput, out var period))
            {
                Console.WriteLine("Invalid period format. Use format: YYYY-I or YYYY-II");
                return;
            }

            Console.WriteLine("Available Internship Types:");
            Console.WriteLine("1. ENGINEERING");
            Console.WriteLine("2. MINOR");
            Console.WriteLine("3. RESEARCH");
            Console.Write("Select type (1-3): ");
            
            if (!Enum.TryParse<AssignmentType>(Console.ReadLine(), out var type))
            {
                Console.WriteLine("Invalid type selected.");
                return;
            }

            var internships = _service.GetInternshipsByPeriodAndType(period, type);
            
            Console.WriteLine($"\nAvailable Internships for {period}, Type: {type}");
            Console.WriteLine("===========================================");
            
            int counter = 1;
            foreach (var internship in internships)
            {
                Console.WriteLine($"{counter++}. {internship.Organization.Name} - {internship.City}");
                Console.WriteLine($"   Project: {internship.ProjectTitle}");
                Console.WriteLine($"   Short Description: {internship.ShortDescription}");
                Console.WriteLine($"   Date: {DateTimeOffset.FromUnixTimeMilliseconds(internship.DateOfSubmission):yyyy-MM-dd}");
                Console.WriteLine();
            }
            
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private void ViewInternshipDetails()
        {
            Console.Write("Enter Internship ID: ");
            if (!int.TryParse(Console.ReadLine(), out int internshipId))
            {
                Console.WriteLine("Invalid ID format.");
                return;
            }

            var internship = _service.GetInternshipById(internshipId);
            if (internship == null)
            {
                Console.WriteLine("Internship not found.");
                return;
            }

            Console.WriteLine("\n=== INTERNSHIP DETAILS ===");
            Console.WriteLine(internship.GetDetails());
            
            var contactPersons = _service.GetContactPersonsByInternshipId(internshipId);
            Console.WriteLine("\n=== CONTACT PERSONS ===");
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

        private void ApplyForInternship()
        {
            Console.Write("Enter Internship ID to apply: ");
            if (!int.TryParse(Console.ReadLine(), out int internshipId))
            {
                Console.WriteLine("Invalid ID format.");
                return;
            }

            var internship = _service.GetInternshipById(internshipId);
            if (internship == null || !internship.IsActive)
            {
                Console.WriteLine("Internship not available.");
                return;
            }

            Console.WriteLine("Select Assignment Type:");
            Console.WriteLine("1. RESEARCH");
            Console.WriteLine("2. MINOR");
            Console.WriteLine("3. ENGINEERING");
            Console.Write("Choice: ");
            
            if (!Enum.TryParse<AssignmentType>(Console.ReadLine(), out var assignmentType))
            {
                Console.WriteLine("Invalid assignment type.");
                return;
            }

            var assignment = new Assignment(_student, internship, assignmentType);
            
            if (_service.AddAssignment(assignment))
            {
                Console.WriteLine("Application submitted successfully!");
                Console.WriteLine("Please email the assignment form to the coordinator.");
            }
            else
            {
                Console.WriteLine("Failed to apply. You may already have an internship in this period.");
            }
            
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private void ViewMyAssignments()
        {
            var assignments = _service.GetAssignmentsForStudent(_student.StudentNumber);
            
            Console.WriteLine("\n=== MY ASSIGNMENTS ===");
            if (assignments.Count == 0)
            {
                Console.WriteLine("No assignments found.");
            }
            else
            {
                foreach (var assignment in assignments)
                {
                    Console.WriteLine(assignment);
                    Console.WriteLine();
                }
            }
            
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        public void ViewAllOrganizations()
        {
            var organizations = _service.GetAllOrganizations();
            
            Console.WriteLine("\n=== ALL ORGANIZATIONS ===");
            foreach (var org in organizations)
            {
                Console.WriteLine($"ID: {org.OrganizationId}");
                Console.WriteLine($"Name: {org.Name}");
                Console.WriteLine($"Address: {org.Address}");
                Console.WriteLine($"Email: {org.Email}");
                Console.WriteLine($"Contact: {org.ContactPerson?.FullName}");
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
                Console.WriteLine("Invalid ID format.");
                return;
            }

            var internships = _service.GetInternshipsByOrganizationId(organizationId);
            
            Console.WriteLine($"\n=== INTERNSHIPS FOR ORGANIZATION ID: {organizationId} ===");
            foreach (var internship in internships)
            {
                Console.WriteLine($"ID: {internship.InternshipId}");
                Console.WriteLine($"Project: {internship.ProjectTitle}");
                Console.WriteLine($"City: {internship.City}");
                Console.WriteLine($"Period: {internship.Period}");
                Console.WriteLine($"Type: {internship.AssignmentType}");
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
                Console.WriteLine("Invalid period format.");
                return;
            }

            Console.WriteLine("Select Assignment Type:");
            Console.WriteLine("1. RESEARCH");
            Console.WriteLine("2. MINOR");
            Console.WriteLine("3. ENGINEERING");
            Console.Write("Choice: ");
            
            if (!Enum.TryParse<AssignmentType>(Console.ReadLine(), out var type))
            {
                Console.WriteLine("Invalid type.");
                return;
            }

            var internships = _service.GetInternshipsByPeriodAndType(period, type);
            
            Console.WriteLine($"\n=== INTERNSHIPS FOR {period}, TYPE: {type} ===");
            foreach (var internship in internships)
            {
                Console.WriteLine($"ID: {internship.InternshipId}");
                Console.WriteLine($"Organization: {internship.Organization.Name}");
                Console.WriteLine($"Project: {internship.ProjectTitle}");
                Console.WriteLine($"City: {internship.City}");
                Console.WriteLine();
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