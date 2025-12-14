// IRepository.cs
namespace SIS.Repositories
{
    public interface IRepository<T>
    {
        bool Add(T entity);
        bool Update(T entity);
        bool Delete(int id);
        T GetById(int id);
        IEnumerable<T> GetAll();
        bool Exists(int id);
    }
}

//     // Organization-specific interface
//     public interface IOrganizationRepository : IRepository<Organization>
//     {
//         bool ValidateOrganizationData(Organization organization);
//         bool AddInternshipToOrganization(int organizationId, Internship internship);
//         List<Internship> GetInternshipsByOrganizationId(int organizationId);
//     }

//     // Internship-specific interface
//     public interface IInternshipRepository : IRepository<Internship>
//     {
//         List<Internship> GetInternshipsByPeriodAndType(Period period, AssignmentType type);
//         List<Internship> GetAvailableInternshipsForStudent(Student student, Period period);
//         bool WithdrawInternship(int internshipId);
//         bool EnrollStudent(int internshipId, Student student);
//         bool WithdrawStudent(int internshipId, Student student);
//         List<ContactPerson> GetContactPersonsByInternshipId(int internshipId);
//     }

//     // Assignment-specific interface
//     public interface IAssignmentRepository : IRepository<Assignment>
//     {
//         bool AddAssignment(Assignment assignment);
//         bool RemoveAssignment(Assignment assignment);
//         bool GradeAssignment(int studentNumber, double grade);
//         List<Assignment> GetAssignmentsForStudent(int studentNumber);
//         bool HasAssignmentInPeriod(int studentNumber, Period period);
//     }

//     // Student-specific interface
//     public interface IStudentRepository : IRepository<Student>
//     {
//         Student GetByStudentNumber(int studentNumber);
//         bool RegisterStudent(Student student);
//     }
// }