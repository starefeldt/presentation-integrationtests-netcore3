using StudentManagementApi.Domain.Interfaces;
using StudentManagementApi.Domain.Models;

namespace StudentManagementApi.IntegrationTests.Helpers
{
    public class FakeMessageSender : IMessageSender
    {
        private bool _shouldSend;

        public FakeMessageSender(bool shouldSend)
        {
            _shouldSend = shouldSend;
        }

        public bool Send(Message message) => _shouldSend;
    }
}
