using StudentManagementApi.Domain.Models;
using System.Threading.Tasks;

namespace StudentManagementApi.Domain.Interfaces
{
    public interface IStudentRepository
    {
        Task<Student> GetBySocialSecurityNumberAsync(string socialSecurityNumber);
        Task RegisterAsync(Student student);
        Task SaveChangesAsync();
    }
}
