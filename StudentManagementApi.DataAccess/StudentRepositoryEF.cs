using Microsoft.EntityFrameworkCore;
using StudentManagementApi.Domain.Interfaces;
using StudentManagementApi.Domain.Models;
using System;
using System.Threading.Tasks;

namespace StudentManagementApi.DataAccess
{
    public class StudentRepositoryEF : IStudentRepository
    {
        private readonly StudentManagementDbContext _context;

        public StudentRepositoryEF(StudentManagementDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(StudentManagementDbContext));
        }

        public async Task<Student> GetBySocialSecurityNumberAsync(string socialSecurityNumber)
        {
            return await _context.Students
                .SingleOrDefaultAsync(s => s.SocialSecurityNumber == socialSecurityNumber);
        }

        public async Task RegisterAsync(Student student)
        {
            await _context.Students.AddAsync(student);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
