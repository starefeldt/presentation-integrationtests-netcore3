using System;

namespace StudentManagementApi.IntegrationTests.Repositories
{
    public interface IDbTestRepository
    {
        void CreateDatabase();
        void DropDatabase();
        void CreateTables();
        void DropTables();
    }
}