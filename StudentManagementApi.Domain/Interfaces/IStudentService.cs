using StudentManagementApi.Domain.Models;
using System.Threading.Tasks;

namespace StudentManagementApi.Domain.Interfaces
{
    public interface IStudentService
    {
        Task Register(Student student);
        Task<Student> GetBySocialSecurityNumber(string socialSecurityNumber);
    }
}