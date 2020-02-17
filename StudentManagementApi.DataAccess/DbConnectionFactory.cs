using Microsoft.Data.SqlClient;
using StudentManagementApi.Domain.Interfaces;
using System;
using System.Data;

namespace StudentManagementApi.DataAccess
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;
        private SqlConnection _conn;

        public DbConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection OpenConnection()
        {
            if (_conn == null || _conn.State != ConnectionState.Open)
            {
                _conn = new SqlConnection(_connectionString);
                _conn.Open();
            }
            return _conn;
        }

        public bool CanOpenConnection()
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void CloseConnection()
        {
            if(_conn != null || _conn.State == ConnectionState.Closed)
            {
                _conn.Close();
            }
            _conn = null;
        }
    }
}
