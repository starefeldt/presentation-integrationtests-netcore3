using System.Data;

namespace StudentManagementApi.Domain.Interfaces
{
    public interface IDbConnectionFactory
    {
        IDbConnection OpenConnection();
        bool CanOpenConnection();
        void CloseConnection();
        void SetDatabase(string databaseName);
        void RemoveDatabase(string databaseName);
        IDbTransaction BeginTransaction();
        IDbTransaction GetTransaction();
    }
}
