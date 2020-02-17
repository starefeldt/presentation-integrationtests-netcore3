using System.Data;

namespace StudentManagementApi.Domain.Interfaces
{
    public interface IDbConnectionFactory
    {
        IDbConnection OpenConnection();
        bool CanOpenConnection();
        void CloseConnection();
    }
}
