using StudentManagementApi.DataAccess;
using StudentManagementApi.Domain.Models;
using System;
using System.Collections.Generic;

namespace StudentManagementApi.IntegrationTests.Repositories
{
    public class TestRepositoryEF : IDbTestRepository
    {
        private readonly StudentManagementDbContext _dbContext;
        private const string InMemoryDatabaseProviderName = "Microsoft.EntityFrameworkCore.InMemory";

        public TestRepositoryEF(StudentManagementDbContext dbContext)
        {
            if (dbContext.Database.ProviderName != InMemoryDatabaseProviderName)
            {
                throw new InvalidOperationException($"Invalid Database Provider Name: {dbContext.Database.ProviderName}. Expected {InMemoryDatabaseProviderName}");
            }
            _dbContext = dbContext;
        }


        public Student GetStudentById(int id) => _dbContext.Students.Find(id);
        public IEnumerable<Student> GetStudents() => _dbContext.Students;

        public void CreateDatabase() => _dbContext.Database.EnsureDeleted();
        public void DropDatabase() => _dbContext.Database.EnsureDeleted();
        public void CreateTables() => _dbContext.Database.EnsureDeleted();
        public void DropTables() => _dbContext.Database.EnsureDeleted();
    }
}
