using StudentManagementApi.Domain.Models;

namespace StudentManagementApi.Domain.Interfaces
{
    public interface IMessageSender
    {
        bool Send(Message message);
    }
}
