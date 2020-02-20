using Dapper;
using StudentManagementApi.Domain.Interfaces;
using StudentManagementApi.Domain.Models;
using System;
using System.Data;
using System.Threading.Tasks;

namespace StudentManagementApi.DataAccess
{
    public class StudentRepositorySQL : IStudentRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public StudentRepositorySQL(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Student> GetBySocialSecurityNumberAsync(string socialSecurityNumber)
        {
            var query = @"SELECT Id
                            , FirstName
                            , LastName
                            , SocialSecurityNumber
                            , Created
                        FROM Student
                        WHERE SocialSecurityNumber = @SocialSecurityNumber";

            using (var conn = _connectionFactory.OpenConnection())
            {
                return await conn.QuerySingleOrDefaultAsync<Student>(query, new { socialSecurityNumber });
            }
        }

        public async Task RegisterAsync(Student student)
        {
            try
            {
                var query = @"INSERT INTO Student
                            ( FirstName
                            , LastName
                            , SocialSecurityNumber
                            , Created)
                            OUTPUT INSERTED.Id
                            VALUES
                            ( @FirstName
                            , @LastName
                            , @SocialSecurityNumber
                            , @Created)";

                var conn = _connectionFactory.OpenConnection();
                var transaction = _connectionFactory.BeginTransaction();

                student.Id = await conn.ExecuteScalarAsync<int>(query, new
                {
                    student.FirstName,
                    student.LastName,
                    student.SocialSecurityNumber,
                    student.Created
                },
                transaction);
            }
            catch (Exception)
            {
                _connectionFactory.CloseConnection();
                throw;
            }
        }

        public async Task SaveChangesAsync()
        {
            var transaction = _connectionFactory.GetTransaction();
            transaction.Commit();
            _connectionFactory.CloseConnection();
            await Task.CompletedTask;
        }
    }
}
