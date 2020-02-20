using Dapper;
using StudentManagementApi.Domain.Interfaces;
using StudentManagementApi.Domain.Models;
using System.Collections.Generic;
using System.IO;

namespace StudentManagementApi.IntegrationTests.Repositories
{
    public class TestRepositorySQL : IDbTestRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private const string DatabaseName = "StudentManagementApi";

        public TestRepositorySQL(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }


        public Student GetStudentById(int id)
        {
            CloseConnectionWhenInTransaction();

            var query = @"SELECT Id
                            , FirstName
                            , LastName
                            , SocialSecurityNumber
                            , Created
                        FROM Student
                        WHERE Id = @id";

            using var conn = _connectionFactory.OpenConnection();
            return conn.QuerySingleOrDefault<Student>(query, new { id });
        }

        private void CloseConnectionWhenInTransaction()
        {
            if (_connectionFactory.GetTransaction() != null)
            {
                _connectionFactory.CloseConnection();
            }
        }

        public IEnumerable<Student> GetStudents()
        {
            CloseConnectionWhenInTransaction();

            var query = @"SELECT Id
                            , FirstName
                            , LastName
                            , SocialSecurityNumber
                            , Created
                        FROM Student";

            using var conn = _connectionFactory.OpenConnection();
            return conn.Query<Student>(query);
        }

        public void CreateDatabase()
        {
            var sql = GetSqlScript("CreateDatabase");
            ExecuteSql(sql);
            _connectionFactory.SetDatabase(DatabaseName);
        }

        public void DropDatabase()
        {
            var sql = GetSqlScript("DropDatabase");
            ExecuteSql(sql);
            _connectionFactory.RemoveDatabase(DatabaseName);
        }

        public void CreateTables() => ExecuteSql(GetSqlScript("CreateTables"));

        public void DropTables() => ExecuteSql(GetSqlScript("DropTables"));

        private string GetSqlScript(string fileName)
        {
            var basePath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            var sqlFile = Path.Combine(basePath + $"\\Scripts\\{fileName}.sql");
            return File.ReadAllText(sqlFile);
        }

        private void ExecuteSql(string sql)
        {
            using var conn = _connectionFactory.OpenConnection();
            conn.Execute(sql);
        }

        
    }
}
