using Dapper;
using StudentManagementApi.Domain.Interfaces;
using System;
using System.IO;

namespace StudentManagementApi.IntegrationTests.Repositories
{
    public class TestRepositorySQL : IDbTestRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public TestRepositorySQL(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public void CreateDatabase()
        {
            var sql = GetSqlScript("CreateDatabase");
            ExecuteSql(sql);
        }

        public void DropDatabase()
        {
            var sql = GetSqlScript("DropDatabase");
            ExecuteSql(sql);
        }

        public void CreateTables()
        {
            var sql = GetSqlScript("CreateTables");
            ExecuteSql(sql);
        }

        public void DropTables()
        {
            var sql = GetSqlScript("DropTables");
            ExecuteSql(sql);
        }

        private string GetSqlScript(string fileName)
        {
            var basePath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            var sqlFile = Path.Combine(basePath + $"\\Scripts\\{fileName}.sql");
            return File.ReadAllText(sqlFile);
        }

        private void ExecuteSql(string sql)
        {
            using var conn = _dbConnectionFactory.OpenConnection();
            conn.Execute(sql);
        }
    }
}
