using Microsoft.Data.SqlClient;
using StudentManagementApi.Domain.Interfaces;
using System;
using System.Data;
using System.Diagnostics;

namespace StudentManagementApi.DataAccess
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private SqlConnection _conn;
        private string _databaseName;
        private SqlTransaction _transaction;
        private readonly string _connectionString;

        public DbConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void SetDatabase(string databaseName) => _databaseName = databaseName;
        public void RemoveDatabase(string databaseName) => _databaseName = null;

        public IDbConnection OpenConnection()
        {
            if (_conn == null || _conn.State != ConnectionState.Open)
            {
                _conn = new SqlConnection(_connectionString);
                _conn.Open();
            }
            if (_databaseName != null)
            {
                _conn.ChangeDatabase(_databaseName);
            }
            return _conn;
        }

        public IDbTransaction BeginTransaction()
        {
            if (_conn == null || _conn.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection is not open. Can not begin transaction");
            }
            _transaction = _conn.BeginTransaction();
            return _transaction;
        }

        public IDbTransaction GetTransaction() => _transaction;

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
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        public void CloseConnection()
        {
            if (_conn == null)
            {
                return;
            }
            if(_conn.State != ConnectionState.Closed)
            {
                _conn.Close();
            }
            _conn = null;
        }

        
    }
}
