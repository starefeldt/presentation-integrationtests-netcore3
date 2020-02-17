using StudentManagementApi.DataAccess;
using System;

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

        public void CreateDatabase()
        {
            _dbContext.Database.EnsureDeleted();
        }

        public void DropDatabase()
        {
            _dbContext.Database.EnsureDeleted();
        }

        public void CreateTables()
        {
            _dbContext.Database.EnsureDeleted();
        }

        public void DropTables()
        {
            _dbContext.Database.EnsureDeleted();
        }
    }
}
