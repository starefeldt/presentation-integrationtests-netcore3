using Microsoft.EntityFrameworkCore;
using StudentManagementApi.Domain.Models;

namespace StudentManagementApi.DataAccess
{
    public class StudentManagementDbContext : DbContext
    {
        public virtual DbSet<Student> Students { get; set; }

        public StudentManagementDbContext()
        { }

        public StudentManagementDbContext(DbContextOptions<StudentManagementDbContext> options)
            : base(options)
        { }
    }
}
