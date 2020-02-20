using StudentManagementApi.Domain.Models;
using System.Collections.Generic;

namespace StudentManagementApi.IntegrationTests.Repositories
{
    public interface IDbTestRepository
    {
        Student GetStudentById(int id);
        IEnumerable<Student> GetStudents();
        void CreateDatabase();
        void DropDatabase();
        void CreateTables();
        void DropTables();
    }
}